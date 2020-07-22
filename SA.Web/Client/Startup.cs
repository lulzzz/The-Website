using System;
using System.Threading.Tasks;
using System.Linq;

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

using SA.Web.Client.WebSockets;
using SA.Web.Client.Data;

namespace SA.Web.Client
{
    public static class Startup
    {
        public static WebAssemblyHost Host { get; private set; }

        public static async Task Main(string[] args)
        {
            WebAssemblyHostBuilder HostBuilder = WebAssemblyHostBuilder.CreateDefault(args);
            HostBuilder.RootComponents.Add<App>("app");
            //HostBuilder.Services.AddTransient<ConnectionManager>();
            //HostBuilder.Services.AddScoped<StateSocketHandler>();
            HostBuilder.Services.AddScoped<ClientState>();
            //HostBuilder.Services.AddScoped<WebSocketManagerMiddleware>();
            HostBuilder.Services.AddScoped<JSSocketInterface>();
            Host = HostBuilder.Build();
            await Host.RunAsync();
        }
    }
}
