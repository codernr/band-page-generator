using BandPageGenerator.Services.Interfaces;
using Stubble.Core.Interfaces;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BandPageGenerator.Services
{
    public class StubbleViewRenderer : IViewRenderer
    {
        private readonly IAsyncStubbleRenderer renderer;

        public StubbleViewRenderer(IAsyncStubbleRenderer renderer)
        {
            this.renderer = renderer;
        }

        public async Task<string> RenderViewToStringAsync<TModel>(string filePath, TModel model)
        {
            using (StreamReader reader = new StreamReader(filePath, Encoding.UTF8))
            {
                var fileContent = await reader.ReadToEndAsync();

                return await this.renderer.RenderAsync(fileContent, model);
            }
        }
    }
}
