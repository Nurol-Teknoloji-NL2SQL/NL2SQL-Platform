using FluentValidation;
using NL2SQL.CoreBackend.Application.Common.Extensions;
using NL2SQL.CoreBackend.Application.DatabaseConnections.DTOs;

namespace NL2SQL.CoreBackend.Application.DatabaseConnections.Validators;

public sealed class UpdateDatabaseConnectionRequestValidator : AbstractValidator<UpdateDatabaseConnectionRequest>
{
    public UpdateDatabaseConnectionRequestValidator()
    {
        When(x => x.Provider is not null, () =>
        {
            RuleFor(x => x.Provider!)
                .Must(s => DatabaseProviderExtensions.TryParseProvider(s, out _))
                .WithMessage("Geçersiz veritabanı sağlayıcısı.");
        });
        When(x => x.DisplayName is not null, () =>
        {
            RuleFor(x => x.DisplayName!).NotEmpty().MaximumLength(200);
        });
    }
}
