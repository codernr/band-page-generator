using BandPageGenerator.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BandPageGenerator.Services.Interfaces
{
    public interface IFacebookClient
    {
        Task<int> GetPageLikeCountAsync();

        Task<List<FacebookEventModel>> GetPageEventsAsync();

        Task<List<FacebookAlbumPhotosModel>> GetAlbumAsync(string albumId);

        Task<FacebookInstagramMediaModel[]> GetRecentInstagramPhotosAsync();

        Task<string> GetProfilePictureAsync();
    }
}
