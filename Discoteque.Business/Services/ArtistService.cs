using Discoteque.Business.IServices;
using Discoteque.Data.Models;

namespace Discoteque.Business.Services;

public class ArtistService : IArtistService
{

    public static List<Artist> artists = new()
    {
        new Artist{
            Name = "Kurt",
            Label = "Rock"
        },
        new Artist {
            Name = "Foyone",
            Label = "Rap"
        }
    };

    public Task<Artist> CreateArtist(Artist artist)
    {
        artists.Add(artist);
        return Task.FromResult(artist);
    }

    public Task<IEnumerable<Artist>> GetArtistsAsync()
    {
        
        return Task.FromResult<IEnumerable<Artist>>(artists);
    }

    public Task<Artist> GetById(int id)
    {
        throw new NotImplementedException();
    }

    public Task<Artist> UpdateArtist(Artist artist)
    {
        throw new NotImplementedException();
    }

}