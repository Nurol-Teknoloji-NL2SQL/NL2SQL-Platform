using NL2SQL.CoreBackend.Application.Common.Models;
using NL2SQL.CoreBackend.Domain.Enums;

namespace NL2SQL.CoreBackend.Application.Common.Interfaces;

public interface ISqlSecurityValidator
{
    SqlValidationResult Validate(string sql, DatabaseProvider dialect);
}
