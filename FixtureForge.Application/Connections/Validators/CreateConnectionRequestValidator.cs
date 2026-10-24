using FixtureForge.Contracts.Connections;
using FluentValidation;

namespace FixtureForge.Application.Connections.Validators;

public class CreateConnectionRequestValidator : AbstractValidator<CreateConnectionRequest>
{
    public CreateConnectionRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ServerReference).NotEmpty().MaximumLength(300);
        RuleFor(x => x.DatabaseName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Provider).Must(p => p == "SqlServer")
            .WithMessage("İlk sürümde yalnızca 'SqlServer' sağlayıcısı desteklenmektedir.");
    }
}