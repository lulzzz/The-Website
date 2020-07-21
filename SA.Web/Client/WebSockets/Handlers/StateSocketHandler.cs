using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using SA.Web.Client.Data;
using SA.Web.Shared.Data.WebSockets;

namespace SA.Web.Client.WebSockets
{
    public class StateSocketHandler : WebSocketHandler
    {
        private JsonSerializerSettings Settings { get; set; } = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead,
            MissingMemberHandling = MissingMemberHandling.Error
        };

        public StateSocketHandler(ConnectionManager webSocketConnectionManager) : base (webSocketConnectionManager) { }

        public override async Task OnConnected(ClientWebSocket socket)
        {
            await base.OnConnected(socket);
        }

        public override async Task Receive(ClientWebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
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
                    await ((ClientState)Startup.Host.Services.GetService(typeof(ClientState))).NotifyUpdateTimesChange(times, false);
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
