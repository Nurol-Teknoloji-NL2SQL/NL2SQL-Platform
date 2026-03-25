using System.Data.Common;
using System.Diagnostics;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using MySqlConnector;
using Npgsql;
using NL2SQL.CoreBackend.Application.Common.Interfaces;
using NL2SQL.CoreBackend.Application.Common.Models;
using NL2SQL.CoreBackend.Domain.Enums;

namespace NL2SQL.CoreBackend.Infrastructure.Services;

public sealed class SqlExecutionService : ISqlExecutionService
{
    public async Task<bool> TestConnectionAsync(
        string connectionString,
        DatabaseProvider provider,
        CancellationToken ct = default)
    {
        await using var conn = (DbConnection)DbConnectionFactory.Create(provider, connectionString);
        await conn.OpenAsync(ct);
        return true;
    }

    public async Task<SqlExecutionResult> ExecuteReadOnlyAsync(
        string connectionString,
        DatabaseProvider provider,
        string sql,
        int commandTimeoutSeconds,
        CancellationToken ct = default)
    {
        var sw = Stopwatch.StartNew();
        await using var conn = (DbConnection)DbConnectionFactory.Create(provider, connectionString);
        await conn.OpenAsync(ct);

        await ApplyReadOnlySessionAsync(conn, provider, ct);

        var cmd = new CommandDefinition(
            sql,
            commandTimeout: commandTimeoutSeconds,
            cancellationToken: ct);

        try
        {
            var grid = await conn.QueryAsync(cmd);
            var rows = new List<Dictionary<string, object?>>();
            foreach (var row in grid)
            {
                var dict = (IDictionary<string, object>)row;
                rows.Add(dict.ToDictionary(k => k.Key, k => k.Value is DBNull ? null : k.Value));
            }

            sw.Stop();
            return new SqlExecutionResult(true, rows, null, rows.Count, sw.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            sw.Stop();
            return new SqlExecutionResult(false, null, ex.Message, 0, sw.ElapsedMilliseconds);
        }
    }

    private static async Task ApplyReadOnlySessionAsync(DbConnection conn, DatabaseProvider provider, CancellationToken ct)
    {
        try
        {
            switch (conn)
            {
                case NpgsqlConnection:
                    await conn.ExecuteAsync(new CommandDefinition(
                        "SET SESSION CHARACTERISTICS AS TRANSACTION READ ONLY",
                        cancellationToken: ct));
                    break;
                case SqlConnection:
                    await conn.ExecuteAsync(new CommandDefinition(
                        "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED",
                        cancellationToken: ct));
                    break;
                case MySqlConnection:
                    await conn.ExecuteAsync(new CommandDefinition(
                        "SET SESSION TRANSACTION READ ONLY",
                        cancellationToken: ct));
                    break;
                case SqliteConnection:
                    break;
            }
        }
        catch
        {
            // Desteklenmeyen motorlarda sessizce devam
        }
    }
}
