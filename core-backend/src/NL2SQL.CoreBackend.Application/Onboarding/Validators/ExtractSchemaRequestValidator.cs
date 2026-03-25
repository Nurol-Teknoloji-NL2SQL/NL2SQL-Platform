using FluentValidation;
using NL2SQL.CoreBackend.Application.Onboarding.DTOs;

namespace NL2SQL.CoreBackend.Application.Onboarding.Validators;

public sealed class ExtractSchemaRequestValidator : AbstractValidator<ExtractSchemaRequestDto>
{
    public ExtractSchemaRequestValidator()
    {
        RuleFor(x => x.DbId).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ConnectionString).MaximumLength(4000).When(x => !string.IsNullOrEmpty(x.ConnectionString));
    }
}
