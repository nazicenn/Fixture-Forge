using FixtureForge.Contracts.Generation;
using FluentValidation;

namespace FixtureForge.Application.Generation.Validators;

public class CreateGenerationJobRequestValidator : AbstractValidator<CreateGenerationJobRequest>
{
    public CreateGenerationJobRequestValidator()
    {
        RuleFor(x => x.Tables).NotEmpty().WithMessage("En az bir tablo hedefi belirtilmelidir.");

        RuleForEach(x => x.Tables).ChildRules(t =>
        {
            t.RuleFor(x => x.SchemaTableId).NotEmpty();
            t.RuleFor(x => x.TargetRowCount).GreaterThanOrEqualTo(0);
        });
    }
}