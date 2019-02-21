using BandPageGenerator.Models;
using System.Threading.Tasks;

namespace BandPageGenerator.Services.Interfaces
{
    public interface ISpotifyClient
    {
        Task<SpotifyAlbumModel[]> GetAlbumsAsync();
    }
}
