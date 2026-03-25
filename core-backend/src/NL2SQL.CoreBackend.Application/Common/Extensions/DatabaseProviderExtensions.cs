using NL2SQL.CoreBackend.Domain.Enums;

namespace NL2SQL.CoreBackend.Application.Common.Extensions;

public static class DatabaseProviderExtensions
{
    public static bool TryParseProvider(string? value, out DatabaseProvider provider)
    {
        provider = DatabaseProvider.PostgreSQL;
        if (string.IsNullOrWhiteSpace(value))
            return false;

        var s = value.Trim();
        if (Enum.TryParse(s, true, out provider))
            return true;

        provider = s.ToLowerInvariant() switch
        {
            "postgresql" or "postgres" or "pgsql" => DatabaseProvider.PostgreSQL,
            "mssql" or "sqlserver" or "microsoft" => DatabaseProvider.MsSql,
            "mysql" or "mariadb" => DatabaseProvider.MySql,
            "oracle" => DatabaseProvider.Oracle,
            "sqlite" => DatabaseProvider.SqLite,
            _ => default
        };

        return s.ToLowerInvariant() is
            "postgresql" or "postgres" or "pgsql" or
            "mssql" or "sqlserver" or "microsoft" or
            "mysql" or "mariadb" or
            "oracle" or "sqlite";
    }
}
