using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BandPageGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging(logging => logging.AddConsole())
                .BuildServiceProvider();
        }
    }
}
