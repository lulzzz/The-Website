using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Net.WebSockets;

namespace SAWebsite.Client.WebSockets
{
    public class ConnectionManager
    {
        private ConcurrentDictionary<Guid, ClientWebSocket> socketDictionary = new ConcurrentDictionary<Guid, ClientWebSocket>();

        public ClientWebSocket GetSocketById(Guid id)
        {
            return socketDictionary.FirstOrDefault(p => p.Key == id).Value;
        }

        public ConcurrentDictionary<Guid, ClientWebSocket> GetAll()
        {
            return socketDictionary;
        }

        public Guid GetId(ClientWebSocket socket)
        {
            return socketDictionary.FirstOrDefault(p => p.Value == socket).Key;
        }
        public void AddSocket(ClientWebSocket socket)
        {
            socketDictionary.TryAdd(Guid.NewGuid(), socket);
        }

        public async Task RemoveSocket(Guid id)
        {
            socketDictionary.TryRemove(id, out ClientWebSocket socket);
            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client.", CancellationToken.None);
        }
    }
}
