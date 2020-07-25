using System;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using SA.Web.Server.Data;
using SA.Web.Shared.Data.WebSockets;

namespace SA.Web.Server.WebSockets
{
    public class StateSocketHandler : WebSocketHandler
    {
        public StateSocketHandler(ConnectionManager webSocketConnectionManager) : base(webSocketConnectionManager) { }

        public override async Task OnConnected(WebSocket socket)
        {
            await base.OnConnected(socket);
        }

        public override async Task Receive(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            string message = Encoding.UTF8.GetString(buffer).Replace("\0", string.Empty);
            MemoryStream stream = new MemoryStream(buffer);

            if (message.StartsWith("CMD.") && Enum.TryParse(typeof(Commands), message.Replace("CMD.", string.Empty), out object cmd))
            {
                if ((Commands)cmd == Commands.GetUpdateData) await SendMessageAsync(socket, 
                    JsonSerializer.Serialize(ServerState.UpdateTimes, ServerState.UpdateTimes.GetType(), ServerState.jsonoptions));
                else if ((Commands)cmd == Commands.GetRoadmapData) await SendMessageAsync(socket, ServerState.RoadmapData);
                else if ((Commands)cmd == Commands.GetBlogData) await SendMessageAsync(socket, ServerState.BlogData);
                else if ((Commands)cmd == Commands.GetChangelogData) await SendMessageAsync(socket, ServerState.ChangelogData);
                else if ((Commands)cmd == Commands.GetPhotographyData) await SendMessageAsync(socket, ServerState.PhotoData);
                else if ((Commands)cmd == Commands.GetVideographyData) await SendMessageAsync(socket, ServerState.VideoData);
                return;
            }
        }
    }
}
