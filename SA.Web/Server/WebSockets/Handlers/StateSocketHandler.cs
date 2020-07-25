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
            MemoryStream stream = new MemoryStream(buffer);
            try
            {
                Commands? cmd;
                if ((cmd = JsonSerializer.DeserializeAsync<Commands>(stream).Result) != null)
                {
                    if (cmd == Commands.GetUpdateData)                  await SendMessageAsync(socket, Encoding.UTF8.GetString(JsonSerializer.SerializeToUtf8Bytes(ServerState.UpdateTimes)));
                    else if (cmd == Commands.GetRoadmapData)            await SendMessageAsync(socket, Encoding.UTF8.GetString(JsonSerializer.SerializeToUtf8Bytes(ServerState.RoadmapData)));
                    else if (cmd == Commands.GetBlogData)               await SendMessageAsync(socket, Encoding.UTF8.GetString(JsonSerializer.SerializeToUtf8Bytes(ServerState.BlogData)));
                    else if (cmd == Commands.GetChangelogData)          await SendMessageAsync(socket, Encoding.UTF8.GetString(JsonSerializer.SerializeToUtf8Bytes(ServerState.ChangelogData)));
                    else if (cmd == Commands.GetPhotographyData)        await SendMessageAsync(socket, Encoding.UTF8.GetString(JsonSerializer.SerializeToUtf8Bytes(ServerState.PhotoData)));
                    else if (cmd == Commands.GetVideographyData)        await SendMessageAsync(socket, Encoding.UTF8.GetString(JsonSerializer.SerializeToUtf8Bytes(ServerState.VideoData)));
                    return;
                }
            }
            catch (JsonException) { }
        }
    }
}
