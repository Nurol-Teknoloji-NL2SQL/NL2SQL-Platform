using FluentValidation;
using NL2SQL.CoreBackend.Application.Onboarding.DTOs;

namespace NL2SQL.CoreBackend.Application.Onboarding.Validators;

public sealed class RegisterSchemaRequestValidator : AbstractValidator<RegisterSchemaRequestDto>
{
    public RegisterSchemaRequestValidator()
    {
        RuleFor(x => x.DbId).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Tables).NotEmpty();
        RuleForEach(x => x.Tables).ChildRules(table =>
        {
            table.RuleFor(t => t.Name).NotEmpty().MaximumLength(256);
            table.RuleFor(t => t.Columns).NotEmpty();
        });
    }
}
