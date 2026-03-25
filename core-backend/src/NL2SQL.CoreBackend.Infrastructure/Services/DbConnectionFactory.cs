using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using MySqlConnector;
using Npgsql;
using NL2SQL.CoreBackend.Domain.Enums;

namespace NL2SQL.CoreBackend.Infrastructure.Services;

internal static class DbConnectionFactory
{
    public static IDbConnection Create(DatabaseProvider provider, string connectionString)
    {
        return provider switch
        {
            DatabaseProvider.PostgreSQL => new NpgsqlConnection(connectionString),
            DatabaseProvider.MsSql => new SqlConnection(connectionString),
            DatabaseProvider.MySql => new MySqlConnection(connectionString),
            DatabaseProvider.SqLite => new SqliteConnection(connectionString),
            DatabaseProvider.Oracle => throw new NotSupportedException("Oracle bağlantısı henüz desteklenmiyor."),
            _ => new NpgsqlConnection(connectionString)
        };
    }
}
