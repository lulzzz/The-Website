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
            webBuilder.UseKestrel(serverOptions =>
            {
                serverOptions.Listen(IPAddress.Loopback, 5000,
                    listenOptions =>
                    {
                        listenOptions.UseHttps("certificate.pfx", SA.Web.Server.Properties.Resources.CertPass);
                    });
            });

            webBuilder.UseStartup<Startup>();
        });
    }
}
