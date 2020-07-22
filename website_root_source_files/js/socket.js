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

    window.connectServerState = (destination) => 
    {
        clientSocket = new WebSocket(destination);
    
        clientSocket.onopen = (e) => 
        {
            console.log("Connection to the server state successful.");
            GLOBAL.JSSocketInterfaceReference.invokeMethodAsync('SendSocketBuffer', event.data);
        };
    
        clientSocket.onmessage = (event) => 
        {
            console.log(event.data);
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
    }
    
    window.sendToServer = (data) => 
    {
        clientSocket.send(data);
    }
    
})();