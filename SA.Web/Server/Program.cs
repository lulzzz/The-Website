using System;
using System.Net;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace SA.Web.Server
{
    public class Program
    {
        public static void Main(string[] args) => CreateHostBuilder(args).Build().Run();
        public static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.ConfigureKestrel(serverOptions =>
            {
                serverOptions.Limits.MaxConcurrentConnections = 1000;
                serverOptions.Limits.MaxConcurrentUpgradedConnections = 1000;
                serverOptions.Listen(IPAddress.Loopback, 5000,
                    listenOptions =>
                    {
                        listenOptions.UseHttps("certificate.pfx", SA.Web.Server.Properties.Resources.CertPass);
                    });
                serverOptions.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(2);
                serverOptions.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(1);
                serverOptions.Limits.Http2.KeepAlivePingTimeout = TimeSpan.FromMinutes(2);
            });

            webBuilder.UseStartup<Startup>();
        });
    }
}
