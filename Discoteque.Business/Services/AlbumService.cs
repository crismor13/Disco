using Discoteque.Data;
using Discoteque.Data.Models;
using Discoteque.Data.Dto;
using System.Net;
using System.Text.RegularExpressions;
using Discoteque.Business.IServices;

namespace Discoteque.Business.Services;

/// <summary>
/// This is a Album service implementation of <see cref="IAlbumService"/> 
/// </summary>
public class AlbumService : IAlbumService
{
    private readonly IUnitOfWork _unitOfWork;

    public AlbumService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Creates a new <see cref="Album"/> entity in Database. 
    /// </summary>
    /// <param name="album">A new album entity</param>
    /// <returns>The created album with an Id assigned</returns>
    public async Task<BaseMessage<Album>> CreateAlbum(Album album)
    {
        var newAlbum = new Album{
            Name = album.Name,
            ArtistId = album.ArtistId,
            Genre = album.Genre,
            Year = album.Year,
            Cost = album.Cost,
        };

        try
        {
            var artist = await _unitOfWork.ArtistRepository.FindAsync(album.ArtistId);
            if(artist == null || album.Cost < 0 || album.Year < 1905 || album.Year > 2023 || AreForbiddenWordsContained(album.Name))
            {
                return Response<Album>.BuildResponse(HttpStatusCode.BadRequest, BaseMessageStatus.BAD_REQUEST_400);
            }
            
            await _unitOfWork.AlbumRepository.AddAsync(newAlbum);
            await _unitOfWork.SaveAsync();    
        }
        catch (Exception ex)
        {
            return Response<Album>.BuildResponse(HttpStatusCode.InternalServerError, BaseMessageStatus.INTERNAL_SERVER_ERROR_500);
        }

        return Response<Album>.BuildResponse(HttpStatusCode.OK, BaseMessageStatus.OK_200, new(){newAlbum});
    }

    /// <summary>
    /// Finds all albums in the EF DB
    /// </summary>
    /// <param name="areReferencesLoaded">Returns associated artists per album if true</param>
    /// <returns>A <see cref="List" /> of <see cref="Album"/> </returns>
    public async Task<BaseMessage<Album>> GetAlbumsAsync(bool areReferencesLoaded)
    {
        List<Album> albums;
        if(areReferencesLoaded)
        {
            try 
            {
                albums = (List<Album>)await _unitOfWork.AlbumRepository.GetAllAsync(null, x => x.OrderBy(x => x.Id), new Artist().GetType().Name);
            }
            catch (Exception ex)
            {
                return Response<Album>.BuildResponse(HttpStatusCode.InternalServerError, BaseMessageStatus.INTERNAL_SERVER_ERROR_500);
            }
            
        }
        else
        {
            try 
            {
                albums = (List<Album>)await _unitOfWork.AlbumRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                return Response<Album>.BuildResponse(HttpStatusCode.InternalServerError, BaseMessageStatus.INTERNAL_SERVER_ERROR_500);
            }
        }
        
        return Response<Album>.BuildResponse(HttpStatusCode.OK, BaseMessageStatus.OK_200, albums);
    }

    /// <summary>
    /// A list of albums released by a <see cref="Artist.Name"/>
    /// </summary>
    /// <param name="artist">The name of the artist</param>
    /// <returns>A <see cref="List" /> of <see cref="Album"/> </returns>
    public async Task<BaseMessage<Album>> GetAlbumsByArtist(string artist)
    {
        List<Album> albums;        
        
        try
        {
            albums = (List<Album>)await _unitOfWork.AlbumRepository.GetAllAsync(x => x.Artist.Name.ToLower().Equals(artist.ToLower()), x => x.OrderBy(x => x.Id), new Artist().GetType().Name);
        }
        catch (Exception ex)
        {
            return Response<Album>.BuildResponse(HttpStatusCode.InternalServerError, BaseMessageStatus.INTERNAL_SERVER_ERROR_500);
        }

        return Response<Album>.BuildResponse(HttpStatusCode.OK, BaseMessageStatus.OK_200, albums);
    }

    /// <summary>
    /// Returns all albums with the assigned genre
    /// </summary>
    /// <param name="genre">A genre from the <see cref="Genres"/> list</param>
    /// <returns>A <see cref="List" /> of <see cref="Album"/> </returns>
    public async Task<BaseMessage<Album>> GetAlbumsByGenre(Genres genre)
    {
        List<Album> albums;        
        
        try
        {
            albums = (List<Album>)await _unitOfWork.AlbumRepository.GetAllAsync(x => x.Genre.Equals(genre), x => x.OrderBy(x => x.Id), new Artist().GetType().Name);
        }
        catch (Exception ex)
        {
            return Response<Album>.BuildResponse(HttpStatusCode.InternalServerError, BaseMessageStatus.INTERNAL_SERVER_ERROR_500);
        }

        return Response<Album>.BuildResponse(HttpStatusCode.OK, BaseMessageStatus.OK_200, albums);
    }

    /// <summary>
    /// Returns all albums published in the year.
    /// </summary>
    /// <param name="year">A gregorian year between 1900 and current year</param>
    /// <returns>A <see cref="List" /> of <see cref="Album"/> </returns>
    public async Task<BaseMessage<Album>> GetAlbumsByYear(int year)
    {
        List<Album> albums;        
        
        try
        {
            albums = (List<Album>)await _unitOfWork.AlbumRepository.GetAllAsync(x => x.Year == year , x => x.OrderBy(x => x.Id), new Artist().GetType().Name);
        }
        catch (Exception ex)
        {
            return Response<Album>.BuildResponse(HttpStatusCode.InternalServerError, BaseMessageStatus.INTERNAL_SERVER_ERROR_500);
        }

        return Response<Album>.BuildResponse(HttpStatusCode.OK, BaseMessageStatus.OK_200, albums);
    }

    /// <summary>
    /// returns all albums released from initial to max year
    /// </summary>
    /// <param name="initialYear">The initial year, min value 1900</param>
    /// <param name="maxYear">the latest year, max value 2025</param>
    /// <returns>A <see cref="List" /> of <see cref="Album"/> </returns>
    public async Task<BaseMessage<Album>> GetAlbumsByYearRange(int initialYear, int maxYear)
    {
        List<Album> albums;        
        
        try
        {
            albums = (List<Album>)await _unitOfWork.AlbumRepository.GetAllAsync(x => x.Year >= initialYear && x.Year <= maxYear , x => x.OrderBy(x => x.Id), new Artist().GetType().Name);
        }
        catch (Exception ex)
        {
            return Response<Album>.BuildResponse(HttpStatusCode.InternalServerError, BaseMessageStatus.INTERNAL_SERVER_ERROR_500);
        }

        return Response<Album>.BuildResponse(HttpStatusCode.OK, BaseMessageStatus.OK_200, albums);
    }

    /// <summary>
    /// Get an album by its EF DB Identity
    /// </summary>
    /// <param name="id">The unique ID of the element</param>
    /// <returns>A <see cref="Album"/> </returns>
    public async Task<BaseMessage<Album>> GetById(int id)
    {

        List<Album> albumList = new();        
        
        try
        {
            var album = await _unitOfWork.AlbumRepository.FindAsync(id);
            albumList.Add(album);
        }    
        catch (Exception ex)
        {
            return Response<Album>.BuildResponse(HttpStatusCode.InternalServerError, BaseMessageStatus.INTERNAL_SERVER_ERROR_500);
        }

        return Response<Album>.BuildResponse(HttpStatusCode.OK, BaseMessageStatus.OK_200, albumList);
    }

    /// <summary>
    /// Updates the <see cref="Album"/> entity in EF DB
    /// </summary>
    /// <param name="album">The Album entity to update</param>
    /// <returns>The new album with updated fields if successful</returns>
    public async Task<BaseMessage<Album>> UpdateAlbum(Album album)
    {
        try
        {
            await _unitOfWork.AlbumRepository.Update(album);
            await _unitOfWork.SaveAsync();
        }
        catch (Exception ex)
        {
            return Response<Album>.BuildResponse(HttpStatusCode.InternalServerError, BaseMessageStatus.INTERNAL_SERVER_ERROR_500);
        }

        return Response<Album>.BuildResponse(HttpStatusCode.OK, BaseMessageStatus.OK_200, new(){album});
    }


    private static bool AreForbiddenWordsContained(string name)
    {
        var prohibitedWords = new List<string>(){"Revolución", "Poder","Amor","Guerra"};
        return prohibitedWords.Any(keyword => Regex.IsMatch(name, Regex.Escape(keyword), RegexOptions.IgnoreCase));

    }
}