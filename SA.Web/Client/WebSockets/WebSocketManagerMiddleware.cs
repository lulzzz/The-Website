using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Net.WebSockets;

using Newtonsoft.Json;

using SA.Web.Shared;
using SA.Web.Client.Data;

namespace SA.Web.Client.WebSockets
{
    public class WebSocketManagerMiddleware
    {
        public WebSocketHandler SocketHandler { get; private set; } = (StateSocketHandler)Startup.Host.Services.GetService(typeof(StateSocketHandler));
        public ClientWebSocket ClientSocket { get; private set; }
        public JsonSerializerSettings Settings { get; private set; } = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead,
            MissingMemberHandling = MissingMemberHandling.Error
        };

        public async Task Connect(ClientState state)
        {
            await Logger.LogInfo("Connecting to server state...");
            ClientSocket = new ClientWebSocket();
            try
            {
#if DEBUG
                await ClientSocket.ConnectAsync(new Uri("ws://localhost:5000/state"), CancellationToken.None);
#else
                await ClientSocket.ConnectAsync(new Uri("wss://ueesa.net/state"), CancellationToken.None);
#endif
            }
            catch (WebSocketException)
            {
                state.NotifyUserWarn("Connection to the server cannot be established. Running in offline mode.");
            }
            if (ClientSocket.State == WebSocketState.Open)
            {
                await Logger.LogInfo("Connecting to server state successful.");
                await SocketHandler.OnConnected(ClientSocket);
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
            else await Logger.LogInfo("Connecting to server state unsuccessful. Running in offline mode.");
        }

        private async Task Receive(ClientWebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            byte[] buffer = new byte[Globals.MaxWebSocketMessageBufferSize];
            try
            {
                while (socket.State == WebSocketState.Open)
                {
                    WebSocketReceiveResult result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    handleMessage(result, buffer);
                }
            }
            catch (WebSocketException) 
            {
                await SocketHandler.OnDisconnected(socket);
            }
        }
    }
}
