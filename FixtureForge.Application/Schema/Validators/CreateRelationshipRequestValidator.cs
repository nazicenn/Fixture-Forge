using FixtureForge.Contracts.Schema;
using FluentValidation;

namespace FixtureForge.Application.Schema.Validators;

public class CreateRelationshipRequestValidator : AbstractValidator<CreateRelationshipRequest>
{
    public CreateRelationshipRequestValidator()
    {
        RuleFor(x => x.SourceTableId).NotEmpty();
        RuleFor(x => x.SourceColumnId).NotEmpty();
        RuleFor(x => x.TargetTableId).NotEmpty();
        RuleFor(x => x.TargetColumnId).NotEmpty();

        RuleFor(x => x.RelationshipType)
            .NotEmpty()
            .Must(t => t == "FK")
            .WithMessage("İlk sürümde yalnızca 'FK' ilişki tipi desteklenmektedir.");

        RuleFor(x => x)
            .Must(x => x.SourceTableId != x.TargetTableId)
            .WithMessage("Kaynak ve hedef tablo aynı olamaz (self-reference ilk sürümde desteklenmiyor).");
    }
}