using BandPageGenerator.Services;
using BandPageGenerator.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Stubble.Core.Builders;
using Stubble.Core.Interfaces;

namespace BandPageGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging(logging => logging.AddConsole())
                .AddSingleton<IAsyncStubbleRenderer>(new StubbleBuilder().Build())
                .AddSingleton<IViewRenderer, StubbleViewRenderer>()
                .BuildServiceProvider();
        }
    }
}
