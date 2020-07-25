using System;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using Microsoft.JSInterop;

using SA.Web.Client.WebSockets;
using SA.Web.Shared.Data.WebSockets;
using System.Text;
using BSL.FileSystem;

namespace SA.Web.Client.Data
{
    public class ClientState
    {
        private IJSRuntime JSRuntime { get; set; }

        public ClientState(IJSRuntime jsruntime)
        {
            JSRuntime = jsruntime;
        }

        public LastUpdateTimes PreviousLocalUpdateTimes { get; private set; } = null;
        public LastUpdateTimes LocalUpdateTimes { get; private set; } = null;
        public event Action OnUpdateTimesChanged;
        public async Task NotifyUpdateTimesChange(LastUpdateTimes data)
        {
            if (LocalUpdateTimes != data)
            {
                PreviousLocalUpdateTimes = LocalUpdateTimes;
                LocalUpdateTimes = data;
                await SetLocalData<LastUpdateTimes>();
            }
            OnUpdateTimesChanged?.Invoke();
        }

        public BlogData BlogData { get; private set; } = null;
        public event Action OnBlogDataChanged;
        public async Task NotifyBlogDataChange(BlogData data, bool isLocalData)
        {
            if (!isLocalData)
            {
                if (Settings.ShowBlogUpdateNotification)
                {
                    await Logger.LogInfo("The blog section updated to " + LocalUpdateTimes.BlogDataUpdate.ToShortDateString() + " "
                        + LocalUpdateTimes.BlogDataUpdate.ToShortTimeString() + " and will be stored for offline usage.");
                    NotifyUserInfo("The blog section updated to " + LocalUpdateTimes.BlogDataUpdate.ToShortDateString() + " "
                        + LocalUpdateTimes.BlogDataUpdate.ToShortTimeString() + " and will be stored for offline usage.");
                }
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
                if (Settings.ShowChangelogUpdateNotification)
                {
                    await Logger.LogInfo("The changelog section updated to " + LocalUpdateTimes.ChangelogDataUpdate.ToShortDateString() + " "
                        + LocalUpdateTimes.ChangelogDataUpdate.ToShortTimeString() + " and will be stored for offline usage.");
                    NotifyUserInfo("The changelog section updated to " + LocalUpdateTimes.ChangelogDataUpdate.ToShortDateString() + " "
                        + LocalUpdateTimes.ChangelogDataUpdate.ToShortTimeString() + " and will be stored for offline usage.");
                }
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
                if (Settings.ShowRoadmapUpdateNotification)
                {
                    await Logger.LogInfo("The roadmap page updated to " + LocalUpdateTimes.RoadmapDataUpdate.ToShortDateString() + " "
                        + LocalUpdateTimes.RoadmapDataUpdate.ToShortTimeString() + " and will be stored for offline usage.");
                    NotifyUserInfo("The roadmap page updated to " + LocalUpdateTimes.RoadmapDataUpdate.ToShortDateString() + " "
                        + LocalUpdateTimes.RoadmapDataUpdate.ToShortTimeString() + " and will be stored for offline usage.");
                }
                RoadmapData = data;
                await SetLocalData<RoadmapData>();
                if (PreviousRoadmapData != null)
                {
                    ChangedFeatures.Clear();
                    /*
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
                    */
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
                if (Settings.ShowPhotographyUpdateNotification)
                {
                    await Logger.LogInfo("The photography page updated to " + LocalUpdateTimes.PhotographyDataUpdate.ToShortDateString() + " "
                        + LocalUpdateTimes.PhotographyDataUpdate.ToShortTimeString() + " and will be stored for offline usage.");
                    NotifyUserInfo("The photography page updated to " + LocalUpdateTimes.PhotographyDataUpdate.ToShortDateString() + " "
                        + LocalUpdateTimes.PhotographyDataUpdate.ToShortTimeString() + " and will be stored for offline usage.");
                }
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
                if (Settings.ShowVideographyUpdateNotification)
                {
                    await Logger.LogInfo("The videography section updated to " + LocalUpdateTimes.VideographyDataUpdate.ToShortDateString() + " "
                        + LocalUpdateTimes.VideographyDataUpdate.ToShortTimeString() + " and will be stored for offline usage.");
                    NotifyUserInfo("The videography section updated to " + LocalUpdateTimes.VideographyDataUpdate.ToShortDateString() + " "
                        + LocalUpdateTimes.VideographyDataUpdate.ToShortTimeString() + " and will be stored for offline usage.");
                }
                VideographyData = data;
                await SetLocalData<MediaVideographyData>();
            }
            OnVideographyDataChanged?.Invoke();
        }

        // Requests

        /*

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

        */

        public async Task RequestUpdateData(bool getFreshCopy = false)
        {
            if ((((JSSocketInterface)Startup.Host.Services.GetService(typeof(JSSocketInterface))).IsConnected && getFreshCopy))
                await ((JSSocketInterface)Startup.Host.Services.GetService(typeof(JSSocketInterface))).Send(Encoding.UTF8.GetString(JsonSerializer.SerializeToUtf8Bytes(Commands.GetUpdateData)));
            else await GetLocalData<LastUpdateTimes>();
        }

        public async Task RequestBlogData(bool getFreshCopy = false)
        {
            if (((JSSocketInterface)Startup.Host.Services.GetService(typeof(JSSocketInterface))).IsConnected && getFreshCopy)
                await ((JSSocketInterface)Startup.Host.Services.GetService(typeof(JSSocketInterface))).Send(Encoding.UTF8.GetString(JsonSerializer.SerializeToUtf8Bytes(Commands.GetBlogData)));
            else await GetLocalData<BlogData>();
        }

        public async Task RequestChangelogData(bool getFreshCopy = false)
        {
            if (((JSSocketInterface)Startup.Host.Services.GetService(typeof(JSSocketInterface))).IsConnected && getFreshCopy)
                await ((JSSocketInterface)Startup.Host.Services.GetService(typeof(JSSocketInterface))).Send(Encoding.UTF8.GetString(JsonSerializer.SerializeToUtf8Bytes(Commands.GetChangelogData)));
            else await GetLocalData<ChangelogData>();
        }

        public async Task RequestRoadmapData(bool getFreshCopy = false)
        {
            if (((JSSocketInterface)Startup.Host.Services.GetService(typeof(JSSocketInterface))).IsConnected && getFreshCopy)
                await ((JSSocketInterface)Startup.Host.Services.GetService(typeof(JSSocketInterface))).Send(Encoding.UTF8.GetString(JsonSerializer.SerializeToUtf8Bytes(Commands.GetRoadmapData)));
            else await GetLocalData<RoadmapData>();
        }

        public async Task RequestPhotographyData(bool getFreshCopy = false)
        {
            if (((JSSocketInterface)Startup.Host.Services.GetService(typeof(JSSocketInterface))).IsConnected && getFreshCopy)
                await ((JSSocketInterface)Startup.Host.Services.GetService(typeof(JSSocketInterface))).Send(Encoding.UTF8.GetString(JsonSerializer.SerializeToUtf8Bytes(Commands.GetPhotographyData)));
            else await GetLocalData<MediaPhotographyData>();
        }

        public async Task RequestVideographyData(bool getFreshCopy = false) 
        {
            if (((JSSocketInterface)Startup.Host.Services.GetService(typeof(JSSocketInterface))).IsConnected && getFreshCopy)
                await ((JSSocketInterface)Startup.Host.Services.GetService(typeof(JSSocketInterface))).Send(Encoding.UTF8.GetString(JsonSerializer.SerializeToUtf8Bytes(Commands.GetVideographyData)));
            else await GetLocalData<MediaVideographyData>();
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
            OnNotificationsUpdate?.Invoke();
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
                    await JSRuntime.InvokeVoidAsync("setData", t.Name, Encoding.UTF8.GetString(JsonSerializer.SerializeToUtf8Bytes(Settings = defaultData ? new GlobalSettings() : Settings)));
                    break;
                case Type t when t == typeof(LastUpdateTimes):
                    await JSRuntime.InvokeVoidAsync("setData", t.Name, Encoding.UTF8.GetString(JsonSerializer.SerializeToUtf8Bytes(LocalUpdateTimes = defaultData ? new LastUpdateTimes() : LocalUpdateTimes)));
                    break;
                case Type t when t == typeof(BlogData):
                    await JSRuntime.InvokeVoidAsync("setData", t.Name, Encoding.UTF8.GetString(JsonSerializer.SerializeToUtf8Bytes(BlogData = defaultData ? new BlogData() : BlogData)));
                    break;
                case Type t when t == typeof(ChangelogData):
                    await JSRuntime.InvokeVoidAsync("setData", t.Name, Encoding.UTF8.GetString(JsonSerializer.SerializeToUtf8Bytes(ChangelogData = defaultData ? new ChangelogData() : ChangelogData)));
                    break;
                case Type t when t == typeof(RoadmapData):
                    await JSRuntime.InvokeVoidAsync("setData", t.Name, Encoding.UTF8.GetString(JsonSerializer.SerializeToUtf8Bytes(RoadmapData = defaultData ? new RoadmapData() : RoadmapData)));
                    break;
                case Type t when t == typeof(MediaPhotographyData):
                    await JSRuntime.InvokeVoidAsync("setData", t.Name, Encoding.UTF8.GetString(JsonSerializer.SerializeToUtf8Bytes(PhotographyData = defaultData ? new MediaPhotographyData() : PhotographyData)));
                    break;
                case Type t when t == typeof(MediaVideographyData):
                    await JSRuntime.InvokeVoidAsync("setData", t.Name, Encoding.UTF8.GetString(JsonSerializer.SerializeToUtf8Bytes(VideographyData = defaultData ? new MediaVideographyData() : VideographyData)));
                    break;
            }
        }

        public async Task GetLocalData<T>()
        {
            string content = await JSRuntime.InvokeAsync<string>("getData", typeof(T).Name);
            MemoryStream stream = null;
            if (!string.IsNullOrEmpty(content)) new MemoryStream(Encoding.UTF8.GetBytes(content));

            try
            {
                switch (typeof(T))
                {
                    case Type t when t == typeof(GlobalSettings):
                        if (stream != null)
                        {
                            Settings = JsonSerializer.DeserializeAsync<GlobalSettings>(stream).Result;
                            OnSettingsUpdate?.Invoke();
                        }
                        else Settings = new GlobalSettings();
                        break;
                    case Type t when t == typeof(LastUpdateTimes):
                        if (stream != null)
                        {
                            LocalUpdateTimes = JsonSerializer.DeserializeAsync<LastUpdateTimes>(stream).Result;
                            OnUpdateTimesChanged?.Invoke();
                        }
                        else if (((JSSocketInterface)Startup.Host.Services.GetService(typeof(JSSocketInterface))).IsConnected) await RequestUpdateData(true);
                        break;
                    case Type t when t == typeof(BlogData):
                        if (stream != null)
                        {
                            BlogData = JsonSerializer.DeserializeAsync<BlogData>(stream).Result;
                            await NotifyBlogDataChange(BlogData, true);
                        }
                        else if (((JSSocketInterface)Startup.Host.Services.GetService(typeof(JSSocketInterface))).IsConnected) await RequestBlogData(true);
                        break;
                    case Type t when t == typeof(ChangelogData):
                        if (stream != null)
                        {
                            ChangelogData = JsonSerializer.DeserializeAsync<ChangelogData>(stream).Result;
                            await NotifyChangelogDataChange(ChangelogData, true);
                        }
                        else if (((JSSocketInterface)Startup.Host.Services.GetService(typeof(JSSocketInterface))).IsConnected) await RequestChangelogData(true);
                        break;
                    case Type t when t == typeof(RoadmapData):
                        if (stream != null)
                        {
                            RoadmapData = JsonSerializer.DeserializeAsync<RoadmapData>(stream).Result;
                            await NotifyRoadmapCardDataChange(RoadmapData, true);
                        }
                        else if (((JSSocketInterface)Startup.Host.Services.GetService(typeof(JSSocketInterface))).IsConnected) await RequestRoadmapData(true);
                        break;
                    case Type t when t == typeof(MediaPhotographyData):
                        if (stream != null)
                        {
                            PhotographyData = JsonSerializer.DeserializeAsync<MediaPhotographyData>(stream).Result;
                            await NotifyPhotographyDataChange(PhotographyData, true);
                        }
                        else if (((JSSocketInterface)Startup.Host.Services.GetService(typeof(JSSocketInterface))).IsConnected) await RequestPhotographyData(true);
                        break;
                    case Type t when t == typeof(MediaVideographyData):
                        if (stream != null)
                        {
                            VideographyData = JsonSerializer.DeserializeAsync<MediaVideographyData>(stream).Result;
                            await NotifyVideographyDataChange(VideographyData, true);
                        }
                        else if (((JSSocketInterface)Startup.Host.Services.GetService(typeof(JSSocketInterface))).IsConnected) await RequestVideographyData(true);
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
