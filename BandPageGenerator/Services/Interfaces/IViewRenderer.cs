using System.Threading.Tasks;

namespace BandPageGenerator.Services.Interfaces
{
    public interface IViewRenderer
    {
        Task<string> RenderViewToStringAsync<TModel>(string filePath, TModel model);
    }
}
