using Dapper;
using Movies.Application.Database;
using Movies.Application.Models;

namespace Movies.Application.Repositories;

public class MovieRepository(IDbConnectionFactory connectionFactory) : IMovieRepository
{
    private readonly IDbConnectionFactory _connectionFactory = connectionFactory;

    public async Task<bool> CreateAsync(Movie movie)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        using var transaction =connection.BeginTransaction();

        var result = await connection.ExecuteAsync(new CommandDefinition("""
                                                                         insert into movies (id, slug, title, yearofrelease)
                                                                             values (@id, @slug, @title, @yearofrelease)
                                                                         """, movie));

        if (result > 0)
        {
            foreach (var genre in movie.Genres)
            {
                await connection.ExecuteAsync(new CommandDefinition("""
                                                                    insert into genres (movieid, name) values (@movieId, @Name)
                                                                    """, new { MovieId = movie.Id, Name = genre}));
            }
        }
        transaction.Commit();
        return result > 0;
    }

    public async Task<IEnumerable<Movie>> GetAllAsync()
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var result = await connection.QueryAsync<Movie>(
            new CommandDefinition("""
                                  select m.*, string_agg(g.name, ',') as genre
                                  from movies m left join genres g on m.id = g.movieid
                                  group by id
                                  """));

        return result.Select(x => new Movie
        {
            Id = x.Id,
            Title = x.Title,
            YearOfRelease = x.YearOfRelease,
            Genres = Enumerable.ToList(x.Genres)
        });
    }

    public async Task<Movie?> GetByIdAsync(Guid id)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var movie = await connection.QuerySingleOrDefaultAsync<Movie>(
            new CommandDefinition("""
                                  select * from movies where id = @id
                                  """, new { id}));

        if (movie is null)
        {
            return null;
        }

        var genres = await connection.QueryAsync<string>(new CommandDefinition("""
            select name from genres where movieid = @id
            """, new { id }));

        foreach (var genre in genres)
        {
            movie.Genres.Add(genre);
        }
        return movie;
    }
    
    public async Task<Movie?> GetBySlugAsync(string slug)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        var movie = await connection.QuerySingleOrDefaultAsync<Movie>(
            new CommandDefinition("""
                                  select * from movies where slug = @slug
                                  """, new { slug }));

        if (movie is null)
        {
            return null;
        }

        var genres = await connection.QueryAsync<string>(
            new CommandDefinition("""
            select name from genres where movieid = @id
            """, new { id = movie.Id }));

        foreach (var genre in genres)
        {
            movie.Genres.Add(genre);
        }
        return movie;
    }

    public async Task<bool> UpdateAsync(Movie movie)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();

        await connection.ExecuteAsync(
            new CommandDefinition("""
                                  delete from genres where movieid = @movieid
                                  """, new { movieid = movie.Id }));

        foreach (var genre in movie.Genres)
        {
            await connection.ExecuteAsync(
                new CommandDefinition("""
                                      insert into genres values (@movieid, @name)
                                      """, new { movieid = movie.Id, name = genre }));
        }

        var result = await connection.ExecuteAsync(
            new CommandDefinition("""
                                  update movies set title = @title, yearofrelease = @yearofrelease, slug = @slug
                                  where id = @id
                                  """, movie));
        transaction.Commit();
        return result > 0;
    }

    public async Task<bool> DeleteByIdAsync(Guid id)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();

        await connection.ExecuteAsync(
            new CommandDefinition("""
                                  delete from genres where movieid = @id
                                  """, new { id }));
        
        await connection.ExecuteAsync(
            new CommandDefinition("""
                delete from movies where id = @id
                """, new { id }));
        
        transaction.Commit();
        return true;
    }

    public async Task<bool> ExistByIdAsync(Guid id)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        return await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition("""
                                  select count(*) from movies where id = @id
                                  """, new { id }));
    }
}