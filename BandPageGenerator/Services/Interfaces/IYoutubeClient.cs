using BandPageGenerator.Models;
using System.Threading.Tasks;

namespace BandPageGenerator.Services.Interfaces
{
    public interface IYoutubeClient
    {
        Task<long> GetCumulatedViewCount();

        Task<YoutubeVideoModel[]> GetFeaturedVideos();
    }
}
