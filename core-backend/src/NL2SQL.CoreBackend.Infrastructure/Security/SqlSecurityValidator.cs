using System.Text.RegularExpressions;
using SqlParser.Net;
using SqlParser.Net.Ast.Expression;
using NL2SQL.CoreBackend.Application.Common.Interfaces;
using NL2SQL.CoreBackend.Application.Common.Models;
using NL2SQL.CoreBackend.Domain.Enums;
using ParserDbType = SqlParser.Net.DbType;

namespace NL2SQL.CoreBackend.Infrastructure.Security;

public sealed class SqlSecurityValidator : ISqlSecurityValidator
{
    private static readonly Regex DangerousPattern = new(
        @"\b(pg_sleep|waitfor\s+delay|benchmark\s*\(|xp_cmdshell|sp_executesql|into\s+outfile|load_file\s*\(|copy\s+to|pg_read_file)\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private static readonly Regex DmlDdlKeywords = new(
        @"\b(INSERT|UPDATE|DELETE|MERGE|TRUNCATE|DROP|ALTER|CREATE|GRANT|REVOKE|EXEC|EXECUTE)\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

    public SqlValidationResult Validate(string sql, DatabaseProvider dialect)
    {
        if (string.IsNullOrWhiteSpace(sql))
            return new SqlValidationResult(false, "SQL_EMPTY", "SQL metni boş olamaz.");

        var trimmed = sql.Trim().TrimEnd(';').Trim();
        if (string.IsNullOrEmpty(trimmed))
            return new SqlValidationResult(false, "SQL_EMPTY", "SQL metni boş olamaz.");

        if (DangerousPattern.IsMatch(trimmed))
            return new SqlValidationResult(false, "SQL_BLOCKED", "Güvenlik nedeniyle engellenen ifade tespit edildi.");

        if (DmlDdlKeywords.IsMatch(trimmed))
            return new SqlValidationResult(false, "SQL_DML_DDL", "DML/DDL komutlarına izin verilmez.");

        if (LooksLikeMultipleStatements(trimmed))
            return new SqlValidationResult(false, "SQL_MULTI", "Birden fazla SQL ifadesine izin verilmez.");

        SqlExpression? ast;
        try
        {
            ast = DbUtils.Parse(trimmed, MapParserDbType(dialect));
        }
        catch (Exception ex)
        {
            return new SqlValidationResult(false, "SQL_PARSE", $"SQL ayrıştırılamadı: {ex.Message}");
        }

        if (ast is null)
            return new SqlValidationResult(false, "SQL_PARSE", "SQL ayrıştırılamadı.");

        if (ast is not SqlSelectExpression)
            return new SqlValidationResult(false, "SQL_NOT_SELECT", "Yalnızca SELECT sorgularına izin verilir.");

        return new SqlValidationResult(true, null, null);
    }

    private static bool LooksLikeMultipleStatements(string sql)
    {
        var parts = sql.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return parts.Length > 1;
    }

    private static ParserDbType MapParserDbType(DatabaseProvider p) => p switch
    {
        DatabaseProvider.PostgreSQL => ParserDbType.Pgsql,
        DatabaseProvider.MsSql => ParserDbType.SqlServer,
        DatabaseProvider.MySql => ParserDbType.MySql,
        DatabaseProvider.Oracle => ParserDbType.Oracle,
        DatabaseProvider.SqLite => ParserDbType.Sqlite,
        _ => ParserDbType.Pgsql
    };
}
