using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Net.WebSockets;

using SA.Web.Shared;
using SA.Web.Client.Data;

namespace SA.Web.Client.WebSockets
{
    public class WebSocketManagerMiddleware
    {
        public WebSocketHandler SocketHandler { get; private set; } = (StateSocketHandler)Startup.Host.Services.GetService(typeof(StateSocketHandler));
        public ClientWebSocket ClientSocket { get; private set; }
        public bool IsConnected { get; private set; }
        public event Action OnServerConnected;

        public async Task Connect(ClientState state)
        {
            await Logger.LogInfo("Connecting to server...");
            ClientSocket = new ClientWebSocket();
            try
            {
#if DEBUG
                await ClientSocket.ConnectAsync(new Uri("ws://localhost:5000/state"), CancellationToken.None).ContinueWith(async (task) => await Continued());
#else
                await ClientSocket.ConnectAsync(new Uri("wss://ueesa.net/state"), CancellationToken.None).ContinueWith(async (task) => await Continued());
#endif

                async Task Continued()
                {
                    if (ClientSocket.State == WebSocketState.Open)
                    {
                        await SocketHandler.OnConnected(ClientSocket);
                        IsConnected = true;
                        OnServerConnected?.Invoke();
                        await Logger.LogInfo("Connection to server was successful.");
                        await Receive(ClientSocket, async (result, buffer) =>
                        {
                            if (result.MessageType == WebSocketMessageType.Text)
                            {
                                await SocketHandler.Receive(ClientSocket, result, buffer);
                                return;
                            }
                            else if (result.MessageType == WebSocketMessageType.Close)
                            {
                                await SocketHandler.OnDisconnected(ClientSocket);
                                return;
                            }
                        });
                    }
                    else if (ClientSocket.State == WebSocketState.None || 
                        ClientSocket.State == WebSocketState.Closed ||
                        ClientSocket.State == WebSocketState.Aborted) await Logger.LogInfo("Connection to the server cannot be established. Running in offline mode.");
                }
            }
            catch (WebSocketException)
            {
                await Logger.LogInfo("Connection to the server cannot be established. Running in offline mode.");
                state.NotifyUserWarn("Connection to the server cannot be established. Running in offline mode.");
            }
        }

        private async Task Receive(ClientWebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[Globals.MaxSocketBufferSize]);
            try
            {
                while (socket.State == WebSocketState.Open)
                {
                    WebSocketReceiveResult result = await socket.ReceiveAsync(buffer, CancellationToken.None);
                    handleMessage(result, buffer.ToArray());
                }
            }
            catch (WebSocketException) 
            {
                await SocketHandler.OnDisconnected(socket);
            }
        }
    }
}
