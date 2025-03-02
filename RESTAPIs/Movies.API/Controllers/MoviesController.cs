using Microsoft.AspNetCore.Mvc;
using Movies.API.Mapping;
using Movies.Application.Models;
using Movies.Application.Repositories;
using Movies.Contracts.Requests;

namespace Movies.API.Controllers;

[ApiController]
[Route("api")]
public class MoviesController : ControllerBase
{
    private readonly IMovieRepository _movieRepository;

    public MoviesController(IMovieRepository movieRepository)
    {
        _movieRepository = movieRepository;
    }

    [HttpPost("movies")]
    public async Task<IActionResult> CreateMovieAsync(CreateMovieRequest request)
    {
        var movie = request.MapToMovie();//Moved the mapping logic to Contract Mapper
        await _movieRepository.CreateAsync(movie);
        //return Ok(movie);
        //Better to return Created and we should not return Ok
        return Created($"/api/movies/{movie.Id}", movie);
    }
}