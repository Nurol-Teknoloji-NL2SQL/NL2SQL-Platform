using System.Data.Common;
using NL2SQL.CoreBackend.Domain.Enums;

namespace NL2SQL.CoreBackend.Application.Common.Extensions;

/// <summary>
/// Converts an ADO.NET-style connection string (the format the .NET stack
/// natively understands and tests against) into a SQLAlchemy URL that the
/// Python AI backend can pass into <c>create_engine()</c>.
///
/// Why this exists: the same connection details have two different
/// syntaxes. ADO.NET uses <c>Host=...;Port=...;Username=...;Password=...</c>
/// (key=value pairs), while SQLAlchemy expects an RFC-3986-style URL such as
/// <c>postgresql+psycopg2://user:pass@host:port/db</c>. We store the ADO.NET
/// form (because Npgsql / EF Core need it) and translate on the fly when
/// delegating schema work to the AI backend.
/// </summary>
public static class SqlAlchemyUrlBuilder
{
    /// <summary>
    /// Builds a SQLAlchemy URL for the given provider. Throws
    /// <see cref="NotSupportedException"/> if the provider has no driver
    /// available in the current ai-backend image, or
    /// <see cref="ArgumentException"/> if mandatory pieces are missing.
    /// </summary>
    public static string Build(DatabaseProvider provider, string adoConnectionString)
    {
        if (string.IsNullOrWhiteSpace(adoConnectionString))
            throw new ArgumentException("Bağlantı dizisi boş.", nameof(adoConnectionString));

        // If the caller already gave us a SQLAlchemy URL (anything that looks
        // like a scheme://...) just pass it through unchanged. This lets power
        // users store the URL form directly if they prefer.
        if (LooksLikeUrl(adoConnectionString))
            return adoConnectionString;

        var builder = new DbConnectionStringBuilder { ConnectionString = adoConnectionString };

        return provider switch
        {
            DatabaseProvider.PostgreSQL => BuildPostgres(builder),
            DatabaseProvider.MsSql or
            DatabaseProvider.MySql or
            DatabaseProvider.Oracle or
            DatabaseProvider.SqLite =>
                throw new NotSupportedException(
                    $"{provider} sağlayıcısı için AI backend'de yüklü bir sürücü yok. " +
                    "Şu an yalnızca PostgreSQL desteklenmektedir."),
            _ => throw new NotSupportedException($"Bilinmeyen sağlayıcı: {provider}")
        };
    }

    private static string BuildPostgres(DbConnectionStringBuilder b)
    {
        var host = Get(b, "Host", "Server", "Data Source");
        var port = Get(b, "Port") ?? "5432";
        var database = Get(b, "Database", "Initial Catalog");
        var user = Get(b, "Username", "User Id", "User", "Uid");
        var password = Get(b, "Password", "Pwd");

        if (string.IsNullOrWhiteSpace(host))
            throw new ArgumentException("Bağlantı dizisinde 'Host' anahtarı bulunamadı.");
        if (string.IsNullOrWhiteSpace(database))
            throw new ArgumentException("Bağlantı dizisinde 'Database' anahtarı bulunamadı.");
        if (string.IsNullOrWhiteSpace(user))
            throw new ArgumentException("Bağlantı dizisinde 'Username' anahtarı bulunamadı.");

        // Userinfo segment (user[:password]) must be percent-encoded so that
        // characters like ':', '@', '/' in the password don't break the URL.
        var userInfo = Uri.EscapeDataString(user);
        if (!string.IsNullOrEmpty(password))
            userInfo += ":" + Uri.EscapeDataString(password);

        return $"postgresql+psycopg2://{userInfo}@{host}:{port}/{Uri.EscapeDataString(database)}";
    }

    /// <summary>
    /// DbConnectionStringBuilder lower-cases all keys, so we look up
    /// case-insensitively across the common aliases each provider uses.
    /// </summary>
    private static string? Get(DbConnectionStringBuilder b, params string[] keys)
    {
        foreach (var key in keys)
        {
            if (b.TryGetValue(key, out var value) && value is not null)
            {
                var s = value.ToString();
                if (!string.IsNullOrWhiteSpace(s))
                    return s;
            }
        }
        return null;
    }

    private static bool LooksLikeUrl(string s)
    {
        // Cheap pre-check: a SQLAlchemy URL always has "://" and never has
        // "=" in its scheme prefix. ADO.NET strings always start with a key.
        var schemeEnd = s.IndexOf("://", StringComparison.Ordinal);
        if (schemeEnd <= 0) return false;
        var scheme = s[..schemeEnd];
        return !scheme.Contains('=') && !scheme.Contains(';');
    }
}
