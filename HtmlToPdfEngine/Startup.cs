using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using DinkToPdf;
using DinkToPdf.Contracts;
using HtmlToPdfEngine;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
[assembly: FunctionsStartup(typeof(Startup))]

namespace HtmlToPdfEngine
{
    public class Startup : FunctionsStartup
    {
        private readonly static string[] SoLibs = new string[] {
             "libbsd.so.0",
             "libcrypto.so.1",
             "libfontconfig.so.1",
             "libfreetype.so.6",
             "libjpeg.so.8",
             "libpng16.so.16",
             "libssl.so.1",
             "libuuid.so.1",
             "libX11.so.6",
             "libXau.so.6",
             "libxcb.so.1",
             "libXdmcp.so.6",
             "libXrender.so.1"
            };
        static Startup()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                string appDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);             
                foreach (var so in SoLibs)
                {
                    NativeLibrary.Load(Path.Combine(appDir, so));
                }
            }
        }

        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {            
            builder.ConfigurationBuilder             
                .AddJsonFile(Path.Combine(builder.GetContext().ApplicationRootPath, $"local.settings.json"), optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();            
        }
        public override void Configure(IFunctionsHostBuilder builder)
        {          
            builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));            
        }
    }
}
