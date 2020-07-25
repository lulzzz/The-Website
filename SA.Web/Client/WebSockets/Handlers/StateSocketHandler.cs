using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using SA.Web.Client.Data;
using SA.Web.Shared.Data.WebSockets;

namespace SA.Web.Client.WebSockets
{
    public class StateSocketHandler : WebSocketHandler
    {
        public StateSocketHandler(ConnectionManager webSocketConnectionManager) : base (webSocketConnectionManager) { }

        public override async Task OnConnected(ClientWebSocket socket)
        {
            await base.OnConnected(socket);
        }

        public override async Task Receive(ClientWebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            MemoryStream stream = new MemoryStream(buffer);
            try
            {
                Commands? cmd;
                if ((cmd = JsonSerializer.DeserializeAsync<Commands>(stream).Result) != null)
                {
                    return;
                }
            }
            catch (JsonException) { }
            try
            {
                LastUpdateTimes times;
                if ((times = JsonSerializer.DeserializeAsync<LastUpdateTimes>(stream).Result) != null)
                {
                    await ((ClientState)Startup.Host.Services.GetService(typeof(ClientState))).NotifyUpdateTimesChange(times, false);
                    return;
                }
            }
            catch (JsonException) { }
            try
            {
                RoadmapData roadmapData;
                if ((roadmapData = JsonSerializer.DeserializeAsync<RoadmapData>(stream).Result) != null)
                {
                    await ((ClientState)Startup.Host.Services.GetService(typeof(ClientState))).NotifyRoadmapCardDataChange(roadmapData, false);
                    return;
                }
            }
            catch (JsonException) { }
            try
            {
                BlogData blogData;
                if ((blogData = JsonSerializer.DeserializeAsync<BlogData>(stream).Result) != null)
                {
                    await ((ClientState)Startup.Host.Services.GetService(typeof(ClientState))).NotifyBlogDataChange(blogData, false);
                    return;
                }
            }
            catch (JsonException) { }
            try
            {
                ChangelogData changelogData;
                if ((changelogData = JsonSerializer.DeserializeAsync<ChangelogData>(stream).Result) != null)
                {
                    await ((ClientState)Startup.Host.Services.GetService(typeof(ClientState))).NotifyChangelogDataChange(changelogData, false);
                    return;
                }
            }
            catch (JsonException) { }
            try
            {
                MediaPhotographyData photographyData;
                if ((photographyData = JsonSerializer.DeserializeAsync<MediaPhotographyData>(stream).Result) != null)
                {
                    await ((ClientState)Startup.Host.Services.GetService(typeof(ClientState))).NotifyPhotographyDataChange(photographyData, false);
                    return;
                }
            }
            catch (JsonException) { }
            try
            {
                MediaVideographyData videographyData;
                if ((videographyData = JsonSerializer.DeserializeAsync<MediaVideographyData>(stream).Result) != null)
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
