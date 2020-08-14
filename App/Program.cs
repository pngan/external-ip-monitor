using System.Threading.Tasks;
using Autofac;
using ipmonitor_interface;

namespace NetCore.Docker
{
    class Program
    {
        static async Task<int> Main()
        {
            IContainer container = DependencyRegistration.RegisterDependencies();

            var ipMonitor = container.Resolve<IIpMonitor>();
            await ipMonitor.RunIpMonitor();
            return 0;
        }
    }
}
