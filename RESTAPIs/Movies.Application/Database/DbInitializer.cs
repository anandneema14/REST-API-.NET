using System.Data;
using Dapper;

namespace Movies.Application.Database;

public class DbInitializer(IDbConnectionFactory connectionFactory)
{
    private readonly IDbConnectionFactory _connectionFactory = connectionFactory;

    public async Task InitializeAsync()
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        await connection.ExecuteAsync("""
                                          create table if not exists Movies(
                                              id UUID primary key,
                                              slug TEXT not null,
                                              title TEXT not null,
                                              yearofrelease integer not null);
                                      """);

        // await connection.ExecuteAsync("""
        //                               create unique index if not exists Movies_slug
        //                               on movies.movies
        //                               using btree(slug);");"
        //                               """);
    }
}