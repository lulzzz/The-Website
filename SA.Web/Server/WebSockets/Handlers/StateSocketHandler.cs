using System;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using SA.Web.Server.Data;
using SA.Web.Shared.Data.WebSockets;

namespace SA.Web.Server.WebSockets
{
    public class StateSocketHandler : WebSocketHandler
    {
        private JsonSerializerSettings Settings     { get; set; } = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead,
            MissingMemberHandling = MissingMemberHandling.Error
        };

        public StateSocketHandler(ConnectionManager webSocketConnectionManager) : base(webSocketConnectionManager) { }

        public override async Task OnConnected(WebSocket socket)
        {
            await base.OnConnected(socket);
        }

        public override async Task Receive(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            try
            {
                Commands? cmd;
                if ((cmd = JsonConvert.DeserializeObject<Commands>(message, Settings)) != null)
                {
                    if (cmd == Commands.GetUpdateData)                  await SendMessageAsync(socket, JsonConvert.SerializeObject(ServerState.UpdateTimes, Settings));
                    else if (cmd == Commands.GetRoadmapData)            await SendMessageAsync(socket, JsonConvert.SerializeObject(ServerState.RoadmapData, Settings));
                    else if (cmd == Commands.GetBlogData)               await SendMessageAsync(socket, JsonConvert.SerializeObject(ServerState.BlogData, Settings));
                    else if (cmd == Commands.GetChangelogData)          await SendMessageAsync(socket, JsonConvert.SerializeObject(ServerState.ChangelogData, Settings));
                    else if (cmd == Commands.GetPhotographyData)        await SendMessageAsync(socket, JsonConvert.SerializeObject(ServerState.PhotoData, Settings));
                    else if (cmd == Commands.GetVideographyData)        await SendMessageAsync(socket, JsonConvert.SerializeObject(ServerState.VideoData, Settings));
                    return;
                }
            }
            catch (JsonException) { }
        }
    }
}
