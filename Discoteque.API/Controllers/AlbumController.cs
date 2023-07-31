using System.Data.SqlTypes;
using Discoteque.Business.IServices;
using Discoteque.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace Discoteque.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AlbumController : ControllerBase
{
    private readonly IAlbumService _albumService;

    public AlbumController(IAlbumService albumService)
    {
        _albumService = albumService;
    }

    [HttpGet]
    [Route("GetAlbums")]
    public async Task<IActionResult> GetAlbums(bool areReferencesLoaded = false)
    {
        var messageResponse = await _albumService.GetAlbumsAsync(areReferencesLoaded);
        var responseList = messageResponse.EntitiesList;
        return (responseList.Any() && responseList[0] != null) ?  Ok(messageResponse) : StatusCode(StatusCodes.Status404NotFound,  "There was not album found");
    }

    [HttpGet]
    [Route("GetAlbumById")]
    public async Task<IActionResult> GetById(int id)
    {
        var messageResponse = await _albumService.GetById(id);
        var responseList = messageResponse.EntitiesList;
        return (responseList.Any() && responseList[0] != null) ? Ok(messageResponse) : StatusCode(StatusCodes.Status404NotFound,  "There was not album found with this id");
    }

    [HttpGet]
    [Route("GetAlbumsByYear")]
    public async Task<IActionResult> GetAlbumsByYear(int year)
    {
        var messageResponse = await _albumService.GetAlbumsByYear(year);
        var responseList = messageResponse.EntitiesList;
        return (responseList.Any() && responseList[0] != null) ? Ok(messageResponse) : StatusCode(StatusCodes.Status404NotFound,  "There were not albums found in this year");
    }

    [HttpGet]
    [Route("GetAlbumsByYearRAnge")]
    public async Task<IActionResult> GetAlbumsByYearRange(int initialYear, int yearRange)
    {
        var messageResponse = await _albumService.GetAlbumsByYearRange(initialYear, yearRange);
        var responseList = messageResponse.EntitiesList;
        return (responseList.Any() && responseList[0] != null) ? Ok(messageResponse) : StatusCode(StatusCodes.Status404NotFound,  "There were not albums found in this year range");
    }

    [HttpGet]
    [Route("GetAlbumsByGenre")]
    public async Task<IActionResult> GetAlbumsByGenre(Genres genre)
    {
        var messageResponse = await _albumService.GetAlbumsByGenre(genre);
        var responseList = messageResponse.EntitiesList;
        return (responseList.Any() && responseList[0] != null) ? Ok(messageResponse) : StatusCode(StatusCodes.Status404NotFound,  "There were not albums found in this genre");
    }

    [HttpGet]
    [Route("GetAlbumsByArtist")]
    public async Task<IActionResult> GetAlbumsByArtist(string artist)
    {
        var messageResponse = await _albumService.GetAlbumsByArtist(artist);
        var responseList = messageResponse.EntitiesList;
        return (responseList.Any() && responseList[0] != null) ? Ok(messageResponse) : StatusCode(StatusCodes.Status404NotFound,  "There were not albums by this artist");
    }

    [HttpPost]
    [Route("CreateAlbum")]
    public async Task<IActionResult> CreateAlbumsAsync(Album album)
    {
        var messageResponse = await _albumService.CreateAlbum(album);
        var responseList = messageResponse.EntitiesList;
        return (responseList.Any() && responseList[0] != null) ? Ok(messageResponse) : StatusCode(StatusCodes.Status500InternalServerError,  "Ocurred an error while creating the album");
    }
}