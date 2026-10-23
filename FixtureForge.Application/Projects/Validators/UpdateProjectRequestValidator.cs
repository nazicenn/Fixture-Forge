using FixtureForge.Contracts.Projects;
using FluentValidation;

namespace FixtureForge.Application.Projects.Validators;

public class UpdateProjectRequestValidator : AbstractValidator<UpdateProjectRequest>
{
    public UpdateProjectRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Proje adı zorunludur.")
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(2000);
    }
}