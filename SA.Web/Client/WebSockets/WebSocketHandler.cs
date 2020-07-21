using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Net.WebSockets;
using System.Collections.Concurrent;

using SA.Web.Client.Data;

namespace SA.Web.Client.WebSockets
{
    public abstract class WebSocketHandler
    {
        protected ConnectionManager WebSocketConnectionManager { get; set; }
        public ConcurrentQueue<byte[]> Backlog { get; private set; } = new ConcurrentQueue<byte[]>();

        public WebSocketHandler(ConnectionManager webSocketConnectionManager)
        {
            WebSocketConnectionManager = webSocketConnectionManager;
        }

        public virtual async Task OnConnected(ClientWebSocket socket)
        {
            WebSocketConnectionManager.AddSocket(socket);
            foreach (byte[] msg in Backlog) await SendMessageAsync(socket, msg);
        }

        public virtual async Task OnDisconnected(ClientWebSocket socket)
        {
            await Logger.LogInfo("Connecting to server state...");
            ((ClientState)Startup.Host.Services.GetService(typeof(ClientState))).NotifyUserAlert("The client has lost connection to the server. This could either be the server itself or your network. Attempting to reconnect...");
            for (int i = 0; i < int.MaxValue; i++)
            {
                await Task.Delay(TimeSpan.FromSeconds(5));
                try
                {
                    socket.Dispose();
                    socket = new ClientWebSocket();
#if DEBUG
                    await socket.ConnectAsync(new Uri("ws://localhost:5000/state"), CancellationToken.None);
#else
                    await socket.ConnectAsync(new Uri("wss://ueesa.net/state"), CancellationToken.None);
#endif
                    await OnConnected(socket);
                    await Logger.LogInfo("Connecting to server state successful.");
                    ((ClientState)Startup.Host.Services.GetService(typeof(ClientState))).NotifyUserAlert("The client has successfully reconnected to server. Everything should function as normal again.");
                    break;
                } 
                catch (WebSocketException) { }
                catch (InvalidOperationException) { }
                await Logger.LogInfo("Connecting to server state unsuccessful. Trying again.");
            }
        }

        public async Task SendMessageAsync(ClientWebSocket socket, string message)
        {
            byte[] msg = Encoding.ASCII.GetBytes(message);
            if (socket == null || socket.State != WebSocketState.Open) Backlog.Enqueue(msg);
            else await socket.SendAsync(msg, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public async Task SendMessageAsync(Guid socketId, string message)
        {
            await SendMessageAsync(WebSocketConnectionManager.GetSocketById(socketId), message);
        }

        public async Task SendMessageAsync(ClientWebSocket socket, ArraySegment<byte> msg)
        {
            await socket.SendAsync(msg, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public async Task SendMessageToAllAsync(string message)
        {
            foreach (var pair in WebSocketConnectionManager.GetAll())
            {
                if (pair.Value.State == WebSocketState.Open) await SendMessageAsync(pair.Value, message);
            }
        }

        public abstract Task Receive(ClientWebSocket socket, WebSocketReceiveResult result, byte[] buffer);
    }
}
