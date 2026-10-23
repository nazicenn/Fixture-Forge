using FixtureForge.Contracts.Schema;
using FluentValidation;

namespace FixtureForge.Application.Schema.Validators;

public class UpdateColumnRequestValidator : AbstractValidator<UpdateColumnRequest>
{
    public UpdateColumnRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Sütun adı zorunludur.")
            .MaximumLength(200)
            .Matches("^[A-Za-z_][A-Za-z0-9_]*$")
            .WithMessage("Sütun adı harf veya _ ile başlamalı.");

        RuleFor(x => x.DataType)
            .NotEmpty()
            .Must(t => SupportedDataTypes.All.Contains(t))
            .WithMessage($"Desteklenmeyen veri tipi.");

        RuleFor(x => x.OrdinalPosition).GreaterThanOrEqualTo(0);

        RuleFor(x => x.MaxLength)
            .Must((req, len) => !SupportedDataTypes.RequireMaxLength.Contains(req.DataType) || (len.HasValue && len.Value > 0))
            .WithMessage("Bu veri tipi için MaxLength zorunludur ve 0'dan büyük olmalıdır.");

        RuleFor(x => x.Precision)
            .Must((req, p) => !SupportedDataTypes.RequirePrecision.Contains(req.DataType) || (p.HasValue && p.Value > 0))
            .WithMessage("Bu veri tipi için Precision zorunludur.");
    }
}