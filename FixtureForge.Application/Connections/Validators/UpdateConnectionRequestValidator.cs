using FixtureForge.Contracts.Connections;
using FluentValidation;

namespace FixtureForge.Application.Connections.Validators;

public class UpdateConnectionRequestValidator : AbstractValidator<UpdateConnectionRequest>
{
    public UpdateConnectionRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ServerReference).NotEmpty().MaximumLength(300);
        RuleFor(x => x.DatabaseName).NotEmpty().MaximumLength(200);
    }
}