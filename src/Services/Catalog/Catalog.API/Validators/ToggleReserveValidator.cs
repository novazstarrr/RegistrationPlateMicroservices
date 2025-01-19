using FluentValidation;

namespace Catalog.API.Validators
{
    public class ToggleReserveValidator : AbstractValidator<Guid>
    {
        public ToggleReserveValidator()
        {
            RuleFor(id => id)
                .NotEmpty()
                .WithMessage("Plate ID is required");
        }
    }
} 