using Movies.Application.Models;

namespace Movies.Application.Repositories;

public interface IMovieRepository
{
    //Return True if movie is created, else false
    Task<bool> CreateAsync(Movie movie);
    
    Task<IEnumerable<Movie>> GetAllAsync();
    
    Task<Movie?> GetByIdAsync(Guid id);
    
    Task<Movie?> GetBySlugAsync(string slug);
    
    Task<bool> UpdateAsync(Movie movie);
    
    Task<bool> DeleteByIdAsync(Guid id);
    
    Task<bool> ExistByIdAsync(Guid id);
}