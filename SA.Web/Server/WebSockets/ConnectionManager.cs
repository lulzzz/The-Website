using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Net.WebSockets;

namespace SA.Web.Server.WebSockets
{
    public class ConnectionManager
    {
        private ConcurrentDictionary<Guid, WebSocket> socketDictionary = new ConcurrentDictionary<Guid, WebSocket>();

        public WebSocket GetSocketById(Guid id)
        {
            return socketDictionary.FirstOrDefault(p => p.Key == id).Value;
        }

        public ConcurrentDictionary<Guid, WebSocket> GetAll()
        {
            return socketDictionary;
        }

        public Guid GetId(WebSocket socket)
        {
            return socketDictionary.FirstOrDefault(p => p.Value == socket).Key;
        }

        public async Task AddSocket(WebSocket socket)
        {
            Guid g = Guid.NewGuid();
            await Logger.LogInfo("Connected socket: " + g);
            socketDictionary.TryAdd(g, socket);
        }

        public async Task RemoveSocket(Guid id)
        {
            try
            {
                socketDictionary.TryRemove(id, out WebSocket socket);
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by server.", CancellationToken.None);
                await Logger.LogWarn("Disconnected socket: " + id);
                socket.Dispose();
                GC.Collect();
            }
            catch (WebSocketException)
            {
                await Logger.LogWarn("Disconnected socket prematurely: " + id);
            }
        }
    }
}
