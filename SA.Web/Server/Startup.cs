using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Net.Http.Headers;

using SA.Web.Shared.Data;
using SA.Web.Server.WebSockets;
using SA.Web.Server.Data;
using SA.Web.Shared;

namespace SA.Web.Server
{
    public class Startup
    {
        public static IWebHostEnvironment Enviroment { get; private set; }
        public static IConfiguration Configuration { get; private set; }
        public static IServiceProvider Services { get; private set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            CIGDataCollector.CollectRoadmapData().GetAwaiter().GetResult();
            ServerState.StartDataCollection();
        }

        public void ConfigureServices(IServiceCollection services)
        {
#if !DEBUG
            if (!string.IsNullOrEmpty(SA.Web.Server.Properties.Resources.ApplicationInsightsKey)) services.AddApplicationInsightsTelemetry(SA.Web.Server.Properties.Resources.ApplicationInsightsKey);
#endif
            services.AddRazorPages();
            services.AddRouting();
            services.Configure((StaticFileOptions op) =>
            {
                op.OnPrepareResponse = (StaticFileResponseContext con) =>
                {
                    con.Context.Response.Headers.Remove(HeaderNames.CacheControl);
                };
            });
            services.AddWebSockets((WebSocketOptions options) =>
            {
                options.KeepAliveInterval = TimeSpan.FromMinutes(2);
#if !DEBUG
                options.AllowedOrigins.Add("https://ueesa.net");
                options.AllowedOrigins.Add("wss://ueesa.net");
                options.AllowedOrigins.Add("ueesa.net");
#endif
            });
            services.AddWebSocketManager();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            Services = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope().ServiceProvider;
            Enviroment = env;
            Globals.IsDevelopmentMode = env.IsDevelopment();
            if (Globals.IsDevelopmentMode) app.UseDeveloperExceptionPage();
            else app.UseExceptionHandler("/Error");

            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();
            app.UseWebSockets();
            app.MapWebSocketManager("/state", Services.GetService<StateSocketHandler>());
            app.UseRouting();
            app.Use(async (context, next) =>
            {
                if (AgentUtils.IsMobile(context.Request.Headers[HeaderNames.UserAgent])) context.Response.Redirect("mobile.html");
                else await next();
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });
            Console.Write("\n");
        }
    }
}
