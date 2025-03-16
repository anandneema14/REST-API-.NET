using Microsoft.AspNetCore.Mvc;
using Movies.API.Mapping;
using Movies.Application.Repositories;
using Movies.Contracts.Requests;

namespace Movies.API.Controllers;

[ApiController]
public class MoviesController(IMovieRepository movieRepository) : ControllerBase
{
    [HttpPost(ApiEndPoints.Movies.Create)]
    public async Task<IActionResult> CreateMovieAsync(CreateMovieRequest request)
    {
        var movie = request.MapToMovie();//Moved the mapping logic to Contract Mapper
        await movieRepository.CreateAsync(movie);
        //return Ok(movie);
        //Better to return Created, and we should not return Ok
        return Created($"/{ApiEndPoints.Movies.Create}/{movie.Id}", movie);
        //return CreatedAtAction(nameof(Get), new { idorSlug = movie.Id }, movie);
    }

    [HttpGet(ApiEndPoints.Movies.Get)]
    public async Task<IActionResult> GetByIdAsync([FromRoute] string idOrSlug)
    {
        var movie = Guid.TryParse(idOrSlug, out var id)
            ? await movieRepository.GetByIdAsync(id)
            : await movieRepository.GetBySlugAsync(idOrSlug);
        if (movie is null)
        {
            return NotFound();
        }
        return Ok(movie.MapToResponse());
    }
    
    [HttpGet(ApiEndPoints.Movies.GetAll)]
    public async Task<IActionResult> GetAllMoviesAsync()
    {
        var movie = await movieRepository.GetAllAsync();
        return Ok(movie);
    }

    [HttpPut(ApiEndPoints.Movies.Update)]
    public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] UpdateMovieRequest request)
    {
        var movie = request.MapToMovie(id);
        var updated= await movieRepository.UpdateAsync(movie);
        if (!updated)
        {
            return NotFound();
        }

        var response = movie.MapToResponse();
        return Ok(response);
    }

    [HttpDelete(ApiEndPoints.Movies.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var deleted = await movieRepository.DeleteByIdAsync(id);
        if (!deleted)
        {
            return NotFound();
        }
        return Ok();
    }
}