using FluentValidation;
using NL2SQL.CoreBackend.Application.Common.Extensions;
using NL2SQL.CoreBackend.Application.DatabaseConnections.DTOs;

namespace NL2SQL.CoreBackend.Application.DatabaseConnections.Validators;

public sealed class CreateDatabaseConnectionRequestValidator : AbstractValidator<CreateDatabaseConnectionRequest>
{
    public CreateDatabaseConnectionRequestValidator()
    {
        RuleFor(x => x.DbId).NotEmpty().MaximumLength(100);
        RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ConnectionString).NotEmpty();
        RuleFor(x => x.Provider).NotEmpty()
            .Must(p => DatabaseProviderExtensions.TryParseProvider(p, out _))
            .WithMessage("Geçersiz veritabanı sağlayıcısı.");
    }
}
