using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;

using Microsoft.JSInterop;

using Newtonsoft.Json;

using SA.Web.Client.WebSockets;
using SA.Web.Shared.Data.WebSockets;

namespace SA.Web.Client.Data
{
    public class ClientState
    {
        private IJSRuntime JSRuntime { get; set; }
        private JsonSerializerSettings JSONSettings { get; set; } = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead,
            MissingMemberHandling = MissingMemberHandling.Error
        };

        public ClientState(IJSRuntime jsruntime)
        {
            JSRuntime = jsruntime;
        }

        public LastUpdateTimes UpdateTimes { get; private set; } = null;
        public event Action OnUpdateTimesChanged;
        public async Task NotifyUpdateTimesChange(LastUpdateTimes data, bool isLocalData)
        {
            if (UpdateTimes != data)
            {
                if (!isLocalData)
                {
                    UpdateTimes = data;
                    await SetLocalData<LastUpdateTimes>();
                }
                OnUpdateTimesChanged?.Invoke();
            }
        }

        public BlogData BlogData { get; private set; } = null;
        public event Action OnBlogDataChanged;
        public async Task NotifyBlogDataChange(BlogData data, bool isLocalData)
        {
            if (!isLocalData)
            {
                if (Settings.ShowBlogUpdateNotification && UpdateTimes != null)
                    NotifyUserInfo("The blog section updated to " + UpdateTimes.BlogDataUpdate.ToShortDateString() + " "
                        + UpdateTimes.BlogDataUpdate.ToShortTimeString() + " and will be stored for offline usage.");
                BlogData = data;
                await SetLocalData<BlogData>();
            }
            OnBlogDataChanged?.Invoke();
        }

        public ChangelogData ChangelogData { get; private set; } = null;
        public event Action OnChangelogDataChanged;
        public async Task NotifyChangelogDataChange(ChangelogData data, bool isLocalData)
        {
            if (!isLocalData)
            {
                if (Settings.ShowChangelogUpdateNotification && UpdateTimes != null)
                    NotifyUserInfo("The changelog section updated to " + UpdateTimes.ChangelogDataUpdate.ToShortDateString() + " "
                        + UpdateTimes.ChangelogDataUpdate.ToShortTimeString() + " and will be stored for offline usage.");
                ChangelogData = data;
                await SetLocalData<ChangelogData>();
            }
            OnChangelogDataChanged?.Invoke();
        }

        public RoadmapData PreviousRoadmapData { get; private set; } = null;
        public RoadmapData RoadmapData { get; private set; } = null;
        public List<RoadmapFeature> ChangedFeatures { get; private set; } = new List<RoadmapFeature>();
        public event Action OnRoadmapCardDataChanged;
        public async Task NotifyRoadmapCardDataChange(RoadmapData data, bool isLocalData)
        {
            data.Cards = data.Cards.OrderBy(o => o.MajorVersion).ThenBy(o => o.MinorVersion).Reverse().ToList();
            if (!isLocalData)
            {
                if (RoadmapData != null) PreviousRoadmapData = RoadmapData;
                if (Settings.ShowRoadmapUpdateNotification && UpdateTimes != null)
                    NotifyUserInfo("The roadmap page updated to " + UpdateTimes.RoadmapDataUpdate.ToShortDateString() + " "
                        + UpdateTimes.RoadmapDataUpdate.ToShortTimeString() + " and will be stored for offline usage.");
                RoadmapData = data;
                await SetLocalData<RoadmapData>();
                if (PreviousRoadmapData != null)
                {
                    ChangedFeatures.Clear();
                    foreach (RoadmapCard card in RoadmapData.Cards)
                    {
                        foreach (RoadmapFeature feature in card.VersionFeatures)
                        {
                            if (PreviousRoadmapData.Cards.Any(x => x.MajorVersion == card.MajorVersion && x.MinorVersion == card.MinorVersion))
                            {
                                RoadmapCard equalCard = PreviousRoadmapData.Cards.Find(x => x.MajorVersion == card.MajorVersion && x.MinorVersion == card.MinorVersion);
                                if (card.MajorVersion == equalCard.MajorVersion && card.MinorVersion == equalCard.MinorVersion)
                                {
                                    RoadmapFeature equalFeature = equalCard.VersionFeatures.Find(x => x.Title == feature.Title);
                                    if (feature.Title == equalFeature.Title)
                                    {
                                        if (!(feature.Category == equalFeature.Category &&
                                            feature.Description == equalFeature.Description &&
                                            feature.Status == equalFeature.Status &&
                                            feature.TaskCount == equalFeature.TaskCount &&
                                            feature.TasksCompleted == equalFeature.TasksCompleted)
                                            ) ChangedFeatures.Add(feature);
                                    }
                                    else ChangedFeatures.Add(feature);
                                }
                            }
                        }
                    }
                }
            }
            OnRoadmapCardDataChanged?.Invoke();
        }

        public MediaPhotographyData PhotographyData { get; private set; } = null;
        public event Action OnPhotographyDataChanged;
        public async Task NotifyPhotographyDataChange(MediaPhotographyData data, bool isLocalData)
        {
            data.Photos = data.Photos.OrderBy(o => o.TakenDate).Reverse().ToList();
            if (!isLocalData)
            {
                if (Settings.ShowPhotographyUpdateNotification && UpdateTimes != null)
                    NotifyUserInfo("The photography section updated to " + UpdateTimes.PhotographyDataUpdate.ToShortDateString() + " "
                        + UpdateTimes.PhotographyDataUpdate.ToShortTimeString() + " and will be stored for offline usage.");
                PhotographyData = data;
                await SetLocalData<MediaPhotographyData>();
            }
            OnPhotographyDataChanged?.Invoke();
        }

        public MediaVideographyData VideographyData { get; private set; } = null;
        public event Action OnVideographyDataChanged;
        public async Task NotifyVideographyDataChange(MediaVideographyData data, bool isLocalData)
        {
            if (!isLocalData)
            {
                if (Settings.ShowVideographyUpdateNotification && UpdateTimes != null)
                    NotifyUserInfo("The videography section updated to " + UpdateTimes.VideographyDataUpdate.ToShortDateString() + " "
                        + UpdateTimes.VideographyDataUpdate.ToShortTimeString() + " and will be stored for offline usage.");
                VideographyData = data;
                await SetLocalData<MediaVideographyData>();
            }
            OnVideographyDataChanged?.Invoke();
        }

        // Requests

        public async Task RequestUpdateData(bool forceUpdate = false) =>            await Send(async (ClientWebSocket socket) => 
        {
            if (!forceUpdate && UpdateTimes != null) await GetLocalData<LastUpdateTimes>();
            else await ((StateSocketHandler)Startup.Host.Services.GetService(typeof(StateSocketHandler))).SendMessageAsync(socket,
                JsonConvert.SerializeObject(Commands.GetUpdateData, JSONSettings));
        });

        public async Task RequestBlogData(bool forceUpdate = false) =>              await Send(async (ClientWebSocket socket) =>
        {
            if (!forceUpdate && BlogData != null) await GetLocalData<BlogData>();
            else await ((StateSocketHandler)Startup.Host.Services.GetService(typeof(StateSocketHandler))).SendMessageAsync(socket,
                JsonConvert.SerializeObject(Commands.GetBlogData, JSONSettings));
        });

        public async Task RequestChangelogData(bool forceUpdate = false) =>         await Send(async (ClientWebSocket socket) =>
        {
            if (!forceUpdate && ChangelogData != null) await GetLocalData<ChangelogData>();
            else await ((StateSocketHandler)Startup.Host.Services.GetService(typeof(StateSocketHandler))).SendMessageAsync(socket,
                JsonConvert.SerializeObject(Commands.GetChangelogData, JSONSettings));
        });

        public async Task RequestRoadmapData(bool forceUpdate = false) =>           await Send(async (ClientWebSocket socket) =>
        {
            if (!forceUpdate && RoadmapData != null) await GetLocalData<RoadmapData>();
            else await ((StateSocketHandler)Startup.Host.Services.GetService(typeof(StateSocketHandler))).SendMessageAsync(socket,
                JsonConvert.SerializeObject(Commands.GetRoadmapData, JSONSettings));
        });

        public async Task RequestPhotographyData(bool forceUpdate = false) =>       await Send(async (ClientWebSocket socket) =>
        {
            if (!forceUpdate && PhotographyData != null) await GetLocalData<MediaPhotographyData>();
            else await ((StateSocketHandler)Startup.Host.Services.GetService(typeof(StateSocketHandler))).SendMessageAsync(socket,
                JsonConvert.SerializeObject(Commands.GetPhotographyData, JSONSettings));
        });

        public async Task RequestVideographyData(bool forceUpdate = false) =>       await Send(async (ClientWebSocket socket) =>
        {
            if (!forceUpdate && VideographyData != null) await GetLocalData<MediaVideographyData>();
            else await ((StateSocketHandler)Startup.Host.Services.GetService(typeof(StateSocketHandler))).SendMessageAsync(socket,
                JsonConvert.SerializeObject(Commands.GetVideographyData, JSONSettings));
        });

        private Task Send(Action<ClientWebSocket> act)
        {
            act.Invoke(((WebSocketManagerMiddleware)Startup.Host.Services.GetService(typeof(WebSocketManagerMiddleware))).ClientSocket);
            return Task.CompletedTask;
        }

        // UI State

        public bool IsSettingsPanelVisible = false;
        public event Action OnSettingsPanelVisibilityToggle;
        public void ToggleSettingsPanelVisibility()
        {
            IsSettingsPanelVisible = !IsSettingsPanelVisible;
            OnSettingsPanelVisibilityToggle?.Invoke();
        }

        // Notifications

        public List<NotificationMessage> NotificationMessages { get; private set; } = new List<NotificationMessage>();
        public event Action OnNotificationsUpdate;
        public void NotifyUserInfo(string message)
        {
            if (!NotificationMessages.Any(o => o.Severity == 0 && o.Message == message + "  -  Click to dismiss!")) NotificationMessages.Add(
                new NotificationMessage { Severity = 0, Message = message + "  -  Click to dismiss!" });
        }

        public void NotifyUserWarn(string message)
        {
            if (!NotificationMessages.Any(o => o.Message == message + "  -  Click to dismiss!")) NotificationMessages.Add(
                new NotificationMessage { Severity = 1, Message = message + "  -  Click to dismiss!" });
            OnNotificationsUpdate?.Invoke();
        }

        public void NotifyUserAlert(string message)
        {
            if (!NotificationMessages.Any(o => o.Severity == 2 && o.Message == message + "  -  Click to dismiss!")) NotificationMessages.Add(
                new NotificationMessage { Severity = 2, Message = message + "  -  Click to dismiss!" });
            OnNotificationsUpdate?.Invoke();
        }

        public void DismissNotification(NotificationMessage msg)
        {
            NotificationMessages.Remove(msg);
            OnNotificationsUpdate?.Invoke();
        }

        // LocalStorage

        public GlobalSettings Settings { get; set; } = null;
        public Action OnSettingsUpdate;

        public async Task SetLocalData<T>(bool defaultData = false)
        {
            switch (typeof(T))
            {
                case Type t when t == typeof(GlobalSettings):
                    await JSRuntime.InvokeVoidAsync("setData", t.Name, JsonConvert.SerializeObject(Settings = defaultData ? new GlobalSettings() : Settings, JSONSettings));
                    break;
                case Type t when t == typeof(LastUpdateTimes):
                    await JSRuntime.InvokeVoidAsync("setData", t.Name, JsonConvert.SerializeObject(UpdateTimes = defaultData ? new LastUpdateTimes() : UpdateTimes, JSONSettings));
                    break;
                case Type t when t == typeof(BlogData):
                    await JSRuntime.InvokeVoidAsync("setData", t.Name, JsonConvert.SerializeObject(BlogData = defaultData ? new BlogData() : BlogData, JSONSettings));
                    break;
                case Type t when t == typeof(ChangelogData):
                    await JSRuntime.InvokeVoidAsync("setData", t.Name, JsonConvert.SerializeObject(ChangelogData = defaultData ? new ChangelogData() : ChangelogData, JSONSettings));
                    break;
                case Type t when t == typeof(RoadmapData):
                    await JSRuntime.InvokeVoidAsync("setData", t.Name, JsonConvert.SerializeObject(RoadmapData = defaultData ? new RoadmapData() : RoadmapData, JSONSettings));
                    break;
                case Type t when t == typeof(MediaPhotographyData):
                    await JSRuntime.InvokeVoidAsync("setData", t.Name, JsonConvert.SerializeObject(PhotographyData = defaultData ? new MediaPhotographyData() : PhotographyData, JSONSettings));
                    break;
                case Type t when t == typeof(MediaVideographyData):
                    await JSRuntime.InvokeVoidAsync("setData", t.Name, JsonConvert.SerializeObject(VideographyData = defaultData ? new MediaVideographyData() : VideographyData, JSONSettings));
                    break;
            }
        }

        public async Task GetLocalData<T>()
        {
            try
            {
                switch (typeof(T))
                {
                    case Type t when t == typeof(GlobalSettings):
                        Settings = JsonConvert.DeserializeObject<GlobalSettings>(await JSRuntime.InvokeAsync<string>("getData", t.Name), JSONSettings);
                        OnSettingsUpdate?.Invoke();
                        break;
                    case Type t when t == typeof(LastUpdateTimes):
                        UpdateTimes = JsonConvert.DeserializeObject<LastUpdateTimes>(await JSRuntime.InvokeAsync<string>("getData", t.Name), JSONSettings);
                        await NotifyUpdateTimesChange(UpdateTimes, true);
                        break;
                    case Type t when t == typeof(BlogData):
                        BlogData = JsonConvert.DeserializeObject<BlogData>(await JSRuntime.InvokeAsync<string>("getData", t.Name), JSONSettings);
                        await NotifyBlogDataChange(BlogData, true);
                        break;
                    case Type t when t == typeof(ChangelogData):
                        ChangelogData = JsonConvert.DeserializeObject<ChangelogData>(await JSRuntime.InvokeAsync<string>("getData", t.Name), JSONSettings);
                        await NotifyChangelogDataChange(ChangelogData, true);
                        break;
                    case Type t when t == typeof(RoadmapData):
                        RoadmapData = JsonConvert.DeserializeObject<RoadmapData>(await JSRuntime.InvokeAsync<string>("getData", t.Name), JSONSettings);
                        await NotifyRoadmapCardDataChange(RoadmapData, true);
                        break;
                    case Type t when t == typeof(MediaPhotographyData):
                        PhotographyData = JsonConvert.DeserializeObject<MediaPhotographyData>(await JSRuntime.InvokeAsync<string>("getData", t.Name), JSONSettings);
                        await NotifyPhotographyDataChange(PhotographyData, true);
                        break;
                    case Type t when t == typeof(MediaVideographyData):
                        VideographyData = JsonConvert.DeserializeObject<MediaVideographyData>(await JSRuntime.InvokeAsync<string>("getData", t.Name), JSONSettings);
                        await NotifyVideographyDataChange(VideographyData, true);
                        break;
                }
            }
            catch (ArgumentNullException) { await SetLocalData<T>(true); }
        }
    }

    public class NotificationMessage
    {
        public byte Severity { get; set; }
        public string Message { get; set; }
    }

    public class GlobalSettings
    {
        public bool PlayLoadingAmbientAudio { get; set; } = false;
        public bool PlayBackgroundAmbientAudio { get; set; } = false;

        public bool ShowBlogUpdateNotification { get; set; } = true;
        public bool ShowChangelogUpdateNotification { get; set; } = true;
        public bool ShowRoadmapUpdateNotification { get; set; } = true;
        public bool ShowPhotographyUpdateNotification { get; set; } = true;
        public bool ShowVideographyUpdateNotification { get; set; } = true;
    }
}
