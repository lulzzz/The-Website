﻿using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;

using Microsoft.JSInterop;

using SA.Web.Client.WebSockets;
using SA.Web.Shared.Data.WebSockets;
using SA.Web.Shared;

namespace SA.Web.Client.Data
{
    public class ClientState
    {
        private IJSRuntime JSRuntime { get; set; }
        public static JsonSerializerOptions jsonoptions = new JsonSerializerOptions
        {
            DefaultBufferSize = Globals.MaxSocketBufferSize
        };

        public ClientState(IJSRuntime jsruntime)
        {
            JSRuntime = jsruntime;
        }

        public LastUpdateTimes PreviousLocalUpdateTimes { get; private set; } = null;
        public LastUpdateTimes LocalUpdateTimes { get; private set; } = null;
        public event Action OnUpdateTimesChanged;
        public async Task NotifyUpdateTimesChange(LastUpdateTimes data, bool isLocalData)
        {
            if (!isLocalData)
            {
                PreviousLocalUpdateTimes = LocalUpdateTimes;
                LocalUpdateTimes = data;
                await SetLocalData<LastUpdateTimes>();
            }
            OnUpdateTimesChanged?.Invoke();
        }

        public NewsData NewsData { get; private set; } = null;
        public event Action OnNewsDataChanged;
        public async Task NotifyNewsDataChange(NewsData data, bool isLocalData)
        {
            if (!isLocalData)
            {
                if (Settings.ShowBlogUpdateNotification)
                {
                    await Logger.LogInfo("The news section updated to " + LocalUpdateTimes.NewsDataUpdate.ToShortDateString() + " "
                        + LocalUpdateTimes.NewsDataUpdate.ToShortTimeString() + " and will be stored for offline usage.");
                    NotifyUserInfo("The news section updated to " + LocalUpdateTimes.NewsDataUpdate.ToShortDateString() + " "
                        + LocalUpdateTimes.NewsDataUpdate.ToShortTimeString() + " and will be stored for offline usage.");
                }
                NewsData = data;
                await SetLocalData<NewsData>();
            }
            OnNewsDataChanged?.Invoke();
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
            for (int i = 0; i < data.Cards.Count() - 1; i++) data.Cards[i].Patches = data.Cards[i].Patches.OrderBy(o => o.PatchVersion).ToList();
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

        public async Task RequestUpdateData(bool getFreshCopy = false) =>            await Send(async (ClientWebSocket socket) => 
        {
            if ((((WebSocketManagerMiddleware)Startup.Host.Services.GetService(typeof(WebSocketManagerMiddleware))).IsConnected && getFreshCopy))
                await ((StateSocketHandler)Startup.Host.Services.GetService(typeof(StateSocketHandler))).SendMessageAsync(socket, "CMD." + Commands.GetUpdateData);
            else await GetLocalData<LastUpdateTimes>();
        });

        public async Task RequestNewsData(bool getFreshCopy = false) =>              await Send(async (ClientWebSocket socket) =>
        {
            if ((((WebSocketManagerMiddleware)Startup.Host.Services.GetService(typeof(WebSocketManagerMiddleware))).IsConnected && getFreshCopy))
                await ((StateSocketHandler)Startup.Host.Services.GetService(typeof(StateSocketHandler))).SendMessageAsync(socket, "CMD." + Commands.GetBlogData);
            else await GetLocalData<NewsData>();
        });

        public async Task RequestChangelogData(bool getFreshCopy = false) =>         await Send(async (ClientWebSocket socket) =>
        {
            if ((((WebSocketManagerMiddleware)Startup.Host.Services.GetService(typeof(WebSocketManagerMiddleware))).IsConnected && getFreshCopy))
                await ((StateSocketHandler)Startup.Host.Services.GetService(typeof(StateSocketHandler))).SendMessageAsync(socket, "CMD." + Commands.GetChangelogData);
            else await GetLocalData<ChangelogData>();
        });

        public async Task RequestRoadmapData(bool getFreshCopy = false) =>           await Send(async (ClientWebSocket socket) =>
        {
            if ((((WebSocketManagerMiddleware)Startup.Host.Services.GetService(typeof(WebSocketManagerMiddleware))).IsConnected && getFreshCopy))
                await ((StateSocketHandler)Startup.Host.Services.GetService(typeof(StateSocketHandler))).SendMessageAsync(socket, "CMD." + Commands.GetRoadmapData);
            else await GetLocalData<RoadmapData>();
        });

        public async Task RequestPhotographyData(bool getFreshCopy = false) =>       await Send(async (ClientWebSocket socket) =>
        {
            if ((((WebSocketManagerMiddleware)Startup.Host.Services.GetService(typeof(WebSocketManagerMiddleware))).IsConnected && getFreshCopy))
                await ((StateSocketHandler)Startup.Host.Services.GetService(typeof(StateSocketHandler))).SendMessageAsync(socket, "CMD." + Commands.GetPhotographyData);
            else await GetLocalData<MediaPhotographyData>();
        });

        public async Task RequestVideographyData(bool getFreshCopy = false) =>       await Send(async (ClientWebSocket socket) =>
        {
            if ((((WebSocketManagerMiddleware)Startup.Host.Services.GetService(typeof(WebSocketManagerMiddleware))).IsConnected && getFreshCopy))
                await ((StateSocketHandler)Startup.Host.Services.GetService(typeof(StateSocketHandler))).SendMessageAsync(socket, "CMD." + Commands.GetVideographyData);
            else await GetLocalData<MediaVideographyData>();
        });

        private static Task Send(Action<ClientWebSocket> act)
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
                    await JSRuntime.InvokeVoidAsync("localstorageinterface.setData", t.Name, 
                        JsonSerializer.Serialize(Settings = defaultData ? new GlobalSettings() : Settings, t, jsonoptions));
                    break;
                case Type t when t == typeof(LastUpdateTimes):
                    await JSRuntime.InvokeVoidAsync("localstorageinterface.setData", t.Name, 
                        JsonSerializer.Serialize(LocalUpdateTimes = defaultData ? new LastUpdateTimes() : LocalUpdateTimes, t, jsonoptions));
                    break;
                case Type t when t == typeof(NewsData):
                    await JSRuntime.InvokeVoidAsync("localstorageinterface.setData", t.Name, 
                        JsonSerializer.Serialize(NewsData = defaultData ? new NewsData() : NewsData, t, jsonoptions));
                    break;
                case Type t when t == typeof(ChangelogData):
                    await JSRuntime.InvokeVoidAsync("localstorageinterface.setData", t.Name, 
                        JsonSerializer.Serialize(ChangelogData = defaultData ? new ChangelogData() : ChangelogData, t, jsonoptions));
                    break;
                case Type t when t == typeof(RoadmapData):
                    await JSRuntime.InvokeVoidAsync("localstorageinterface.setData", t.Name, 
                        JsonSerializer.Serialize(RoadmapData = defaultData ? new RoadmapData() : RoadmapData, t, jsonoptions));
                    break;
                case Type t when t == typeof(MediaPhotographyData):
                    await JSRuntime.InvokeVoidAsync("localstorageinterface.setData", t.Name, 
                        JsonSerializer.Serialize(PhotographyData = defaultData ? new MediaPhotographyData() : PhotographyData, t, jsonoptions));
                    break;
                case Type t when t == typeof(MediaVideographyData):
                    await JSRuntime.InvokeVoidAsync("localstorageinterface.setData", t.Name, 
                        JsonSerializer.Serialize(VideographyData = defaultData ? new MediaVideographyData() : VideographyData, t, jsonoptions));
                    break;
            }
        }

        public async Task GetLocalData<T>()
        {
            string content = await JSRuntime.InvokeAsync<string>("localstorageinterface.getData", typeof(T).Name);

            try
            {
                switch (typeof(T))
                {
                    case Type t when t == typeof(GlobalSettings):
                        if (!string.IsNullOrEmpty(content))
                        {
                            Settings = JsonSerializer.Deserialize<GlobalSettings>(content, jsonoptions);
                            OnSettingsUpdate?.Invoke();
                        }
                        else Settings = new GlobalSettings();
                        break;
                    case Type t when t == typeof(LastUpdateTimes):
                        if (!string.IsNullOrEmpty(content))
                        {
                            LocalUpdateTimes = JsonSerializer.Deserialize<LastUpdateTimes>(content, jsonoptions);
                            OnUpdateTimesChanged?.Invoke();
                        }
                        else await RequestUpdateData(true);
                        break;
                    case Type t when t == typeof(NewsData):
                        if (!string.IsNullOrEmpty(content))
                        {
                            NewsData = JsonSerializer.Deserialize<NewsData>(content, jsonoptions);
                            await NotifyNewsDataChange(NewsData, true);
                        }
                        else await RequestNewsData(true);
                        break;
                    case Type t when t == typeof(ChangelogData):
                        if (!string.IsNullOrEmpty(content))
                        {
                            ChangelogData = JsonSerializer.Deserialize<ChangelogData>(content, jsonoptions);
                            await NotifyChangelogDataChange(ChangelogData, true);
                        }
                        else await RequestChangelogData(true);
                        break;
                    case Type t when t == typeof(RoadmapData):
                        if (!string.IsNullOrEmpty(content))
                        {
                            RoadmapData = JsonSerializer.Deserialize<RoadmapData>(content, jsonoptions);
                            await NotifyRoadmapCardDataChange(RoadmapData, true);
                        }
                        else await RequestRoadmapData(true);
                        break;
                    case Type t when t == typeof(MediaPhotographyData):
                        if (!string.IsNullOrEmpty(content))
                        {
                            PhotographyData = JsonSerializer.Deserialize<MediaPhotographyData>(content, jsonoptions);
                            await NotifyPhotographyDataChange(PhotographyData, true);
                        }
                        else await RequestPhotographyData(true);
                        break;
                    case Type t when t == typeof(MediaVideographyData):
                        if (!string.IsNullOrEmpty(content))
                        {
                            VideographyData = JsonSerializer.Deserialize<MediaVideographyData>(content, jsonoptions);
                            await NotifyVideographyDataChange(VideographyData, true);
                        }
                        else await RequestVideographyData(true);
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
