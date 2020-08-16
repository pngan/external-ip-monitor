using ipchange_action;
using ipchange_detector;
using Microsoft.Extensions.DependencyInjection;
using Autofac.Extensions.DependencyInjection;
using Autofac;
using ipmonitor_interface;
using System.IO.Abstractions;

namespace NetCore.Docker
{
    internal class DependencyRegistration
    {
        internal static IContainer RegisterDependencies()
        {
            var services = new ServiceCollection();
            services.AddHttpClient();   // for implementation of IHttpClientFactory

            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(services);
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
