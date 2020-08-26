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
            string message = Encoding.UTF8.GetString(buffer);
            message = message.Replace("\0", string.Empty);
            if (message.StartsWith("CMD.") && Enum.TryParse(typeof(Commands), message.Replace("CMD.", string.Empty), out object cmd))
            {
                //message = message.Replace("CMD.", string.Empty);
                return;
            }
            else if (message.StartsWith("JSON."))
            {
                message = message.Replace("JSON.", string.Empty);
                Type type;

                LastUpdateTimes times;
                if (message.StartsWith((type = typeof(LastUpdateTimes)).Name))
                {
                    message = message.Substring(type.Name.Length);
                    if ((times = JsonSerializer.Deserialize<LastUpdateTimes>(message, ClientState.jsonoptions)) != null)
                    {
                        await ((ClientState)Startup.Host.Services.GetService(typeof(ClientState))).NotifyUpdateTimesChange(times, false);
                        return;
                    }
                }

                RoadmapData roadmapData;
                if (message.StartsWith((type = typeof(RoadmapData)).Name))
                {
                    message = message.Substring(type.Name.Length);
                    if ((roadmapData = JsonSerializer.Deserialize<RoadmapData>(message, ClientState.jsonoptions)) != null)
                    {
                        await ((ClientState)Startup.Host.Services.GetService(typeof(ClientState))).NotifyRoadmapCardDataChange(roadmapData, false);
                        return;
                    }
                }

                NewsData blogData;
                if (message.StartsWith((type = typeof(NewsData)).Name))
                {
                    message = message.Substring(type.Name.Length);
                    if ((blogData = JsonSerializer.Deserialize<NewsData>(message, ClientState.jsonoptions)) != null)
                    {
                        await ((ClientState)Startup.Host.Services.GetService(typeof(ClientState))).NotifyNewsDataChange(blogData, false);
                        return;
                    }
                }

                ChangelogData changelogData;
                if (message.StartsWith((type = typeof(ChangelogData)).Name))
                {
                    message = message.Substring(type.Name.Length);
                    if ((changelogData = JsonSerializer.Deserialize<ChangelogData>(message, ClientState.jsonoptions)) != null)
                    {
                        await ((ClientState)Startup.Host.Services.GetService(typeof(ClientState))).NotifyChangelogDataChange(changelogData, false);
                        return;
                    }
                }

                MediaPhotographyData photographyData;
                if (message.StartsWith((type = typeof(MediaPhotographyData)).Name))
                {
                    message = message.Substring(type.Name.Length);
                    if ((photographyData = JsonSerializer.Deserialize<MediaPhotographyData>(message, ClientState.jsonoptions)) != null)
                    {
                        await ((ClientState)Startup.Host.Services.GetService(typeof(ClientState))).NotifyPhotographyDataChange(photographyData, false);
                        return;
                    }
                }

                MediaVideographyData videographyData;
                if (message.StartsWith((type = typeof(MediaVideographyData)).Name))
                {
                    message = message.Substring(type.Name.Length);
                    if ((videographyData = JsonSerializer.Deserialize<MediaVideographyData>(message, ClientState.jsonoptions)) != null)
                    {
                        await ((ClientState)Startup.Host.Services.GetService(typeof(ClientState))).NotifyVideographyDataChange(videographyData, false);
                        return;
                    }
                }
            }

            return;
        }
    }
}
