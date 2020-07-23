using System;
using System.Threading.Tasks;

using Microsoft.JSInterop;

using Newtonsoft.Json;

using SA.Web.Client.Data;
using SA.Web.Shared.Data.WebSockets;


namespace SA.Web.Client.WebSockets
{
    public class JSSocketInterface
    {
        private JsonSerializerSettings Settings { get; set; } = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead,
            MissingMemberHandling = MissingMemberHandling.Error
        };

        private IJSRuntime runtime = (IJSRuntime)Startup.Host.Services.GetService(typeof(IJSRuntime));
        public bool IsConnected = false;
        public event Action OnServerConnected;

        public JSSocketInterface()
        {
            DotNetObjectReference<JSSocketInterface> lDotNetReference = DotNetObjectReference.Create(this);
            runtime.InvokeVoidAsync("GLOBAL.SetJSSocketInterfaceReference", lDotNetReference);
        }

#if !DEBUG
        public async Task Connect() => await runtime.InvokeVoidAsync("connectServerState", "wss://ueesa.net/state");
#else
        public async Task Connect() => await runtime.InvokeVoidAsync("connectServerState", "ws://localhost:5000/state");
#endif

        public async Task Send(string message) => await runtime.InvokeVoidAsync("sendToServer", message);

        [JSInvokable("SendConnectionActive")]
        public async Task ConnectionActive()
        {
            IsConnected = true;
            OnServerConnected?.Invoke();
        }

        [JSInvokable("SocketReceive")]
        public async Task Receive(string message)
        {
            try
            {
                Commands? cmd;
                if ((cmd = JsonConvert.DeserializeObject<Commands>(message, Settings)) != null)
                {
                    return;
                }
            }
            catch (JsonException) { }
            try
            {
                LastUpdateTimes times;
                if ((times = JsonConvert.DeserializeObject<LastUpdateTimes>(message, Settings)) != null)
                {
                    await ((ClientState)Startup.Host.Services.GetService(typeof(ClientState))).NotifyUpdateTimesChange(times);
                    return;
                }
            }
            catch (JsonException) { }
            try
            {
                RoadmapData roadmapData;
                if ((roadmapData = JsonConvert.DeserializeObject<RoadmapData>(message, Settings)) != null)
                {
                    await ((ClientState)Startup.Host.Services.GetService(typeof(ClientState))).NotifyRoadmapCardDataChange(roadmapData, false);
                    return;
                }
            }
            catch (JsonException) { }
            try
            {
                BlogData blogData;
                if ((blogData = JsonConvert.DeserializeObject<BlogData>(message, Settings)) != null)
                {
                    await ((ClientState)Startup.Host.Services.GetService(typeof(ClientState))).NotifyBlogDataChange(blogData, false);
                    return;
                }
            }
            catch (JsonException) { }
            try
            {
                ChangelogData changelogData;
                if ((changelogData = JsonConvert.DeserializeObject<ChangelogData>(message, Settings)) != null)
                {
                    await ((ClientState)Startup.Host.Services.GetService(typeof(ClientState))).NotifyChangelogDataChange(changelogData, false);
                    return;
                }
            }
            catch (JsonException) { }
            try
            {
                MediaPhotographyData photographyData;
                if ((photographyData = JsonConvert.DeserializeObject<MediaPhotographyData>(message, Settings)) != null)
                {
                    await ((ClientState)Startup.Host.Services.GetService(typeof(ClientState))).NotifyPhotographyDataChange(photographyData, false);
                    return;
                }
            }
            catch (JsonException) { }
            try
            {
                MediaVideographyData videographyData;
                if ((videographyData = JsonConvert.DeserializeObject<MediaVideographyData>(message, Settings)) != null)
                {
                    await ((ClientState)Startup.Host.Services.GetService(typeof(ClientState))).NotifyVideographyDataChange(videographyData, false);
                    return;
                }
            }
            catch (JsonException) { }
            return;
        }
    }
}
