using ipchange_action;
using ipchange_detector;
using Microsoft.Extensions.DependencyInjection;
using Autofac.Extensions.DependencyInjection;
using Autofac;
using ipmonitor_interface;
using System.IO.Abstractions;
using Serilog;
using AutofacSerilogIntegration;

namespace NetCore.Docker
{
    internal class DependencyRegistration
    {
        internal static IContainer RegisterDependencies()
        {
            // Set up SeriLogger
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Seq("http://manuka:5341")
                .WriteTo.Console(theme: Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme.Code)
                .CreateLogger();

            // Set up IHttpClientFactory
            var services = new ServiceCollection();
            services.AddHttpClient();

            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(services);
            containerBuilder.RegisterLogger();
            containerBuilder.RegisterType<IpAddressChangeDetector>().As<IIpAddressChangeDetector>().SingleInstance();
            containerBuilder.RegisterType<IpAddressProcessorEngine>().As<IIpAddressProcessorEngine>().SingleInstance();
            containerBuilder.RegisterType<ChangeOvhRegistryARecord>().As<IIpAddressProcessor>().SingleInstance();
            containerBuilder.RegisterType<IpMonitor>().As<IIpMonitor>().SingleInstance();
            containerBuilder.RegisterType<FileSystem>().As<IFileSystem>().SingleInstance();
            
            var container = containerBuilder.Build();
            return container;
        }
    }
}
