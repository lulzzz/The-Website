var GLOBAL = {};
GLOBAL.JSSocketInterfaceReference = null;
GLOBAL.SetJSSocketInterfaceReference = function (ref) 
{
    if (GLOBAL.JSSocketInterfaceReference === null)
    {
        GLOBAL.JSSocketInterfaceReference = ref;
    } 
};

(function () {

    var clientSocket = undefined;

    window.socketinterface = {

        connectServerState: (destination) => 
        {
            clientSocket = new WebSocket(destination);
        
            clientSocket.onopen = (e) => 
            {
                console.log("Connection to the server state successful.");
                GLOBAL.JSSocketInterfaceReference.invokeMethodAsync('SendConnectionActive', event.data);
            };
        
            clientSocket.onmessage = (event) => 
            {
                GLOBAL.JSSocketInterfaceReference.invokeMethodAsync('SocketReceive', event.data);
            };
        
            clientSocket.onclose = (event) => 
            {
                if (event.wasClean) 
                {
                    console.log(`Connection to the server state has closed successfully.`);
                } 
                else 
                {
        
                    console.log('Connection to the server state has unexpectedly closed!');
                }
            };
        
            clientSocket.onerror = (error) => 
            {
                console.log(`A websocket error has occured: ${error.message}`);
            };
        },
        sendToServer: (data) => 
        {
            clientSocket.send(data);
        }
    }
    
})();