using FluentValidation;
using Microsoft.Extensions.Options;
using NL2SQL.CoreBackend.Application.Common.Options;
using NL2SQL.CoreBackend.Application.Query.DTOs;

namespace NL2SQL.CoreBackend.Application.Query.GenerateSql;

public sealed class GenerateSqlRequestValidator : AbstractValidator<GenerateSqlRequest>
{
    public GenerateSqlRequestValidator(IOptions<QueryExecutionOptions> options)
    {
        var opt = options.Value;

        RuleFor(x => x.Query)
            .NotEmpty()
            .MaximumLength(50_000);

        RuleFor(x => x.DbId)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(x => x.DryRunLimit)
            .InclusiveBetween(1, 10_000)
            .When(x => x.DryRunLimit.HasValue);

        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .Must(x => x is 0 || (x >= 1 && x <= opt.MaxPageSize))
            .WithMessage(_ => $"PageSize 1 ile {opt.MaxPageSize} arasında olmalı veya 0 (varsayılan) için bırakılmalıdır.");
    }
}
