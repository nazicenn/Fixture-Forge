using FixtureForge.Contracts.Schema;
using FluentValidation;

namespace FixtureForge.Application.Schema.Validators;

public class UpdateTableRequestValidator : AbstractValidator<UpdateTableRequest>
{
    public UpdateTableRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tablo adı zorunludur.")
            .MaximumLength(200)
            .Matches("^[A-Za-z_][A-Za-z0-9_]*$")
            .WithMessage("Tablo adı harf veya _ ile başlamalı, sadece harf, rakam ve _ içermelidir.");

        RuleFor(x => x.Description).MaximumLength(2000);
    }
}