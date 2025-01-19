using FluentValidation;
using Catalog.API.DTOs.Requests;
using System.Text.RegularExpressions;

namespace Catalog.API.Validators
{
    public class CreatePlateDtoValidator : AbstractValidator<CreatePlateDto>
    {
        private const int MAX_REGISTRATION_LENGTH = 7;

        public CreatePlateDtoValidator()
        {
            RuleFor(x => x.Registration)
                .NotEmpty()
                .WithMessage("Registration is required")
                .MaximumLength(MAX_REGISTRATION_LENGTH)
                .WithMessage($"Registration cannot exceed {MAX_REGISTRATION_LENGTH} characters")
                .Matches(@"^[A-Z0-9]{1,7}$")
                .WithMessage("Registration must contain only letters and numbers, max 7 characters");

            RuleFor(x => x.PurchasePrice)
                .GreaterThan(0)
                .WithMessage("Purchase price must be greater than 0")
                .LessThanOrEqualTo(1000000)
                .WithMessage("Purchase price cannot exceed 1,000,000");

            RuleFor(x => x.Letters)
                .NotEmpty()
                .WithMessage("Letters are required")
                .MaximumLength(4)
                .WithMessage("Letters cannot exceed 4 characters")
                .Matches(@"^[A-Z]+$")
                .WithMessage("Letters must be uppercase letters only");

            RuleFor(x => x.Numbers)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Numbers must be greater than 0")
                .LessThanOrEqualTo(999)
                .WithMessage("Numbers cannot exceed 999");
        }

        private bool BeValidRegistration(string registration)
        {
            const int MIN_REGISTRATION_LENGTH = 1;

            if (string.IsNullOrEmpty(registration)) return false;
            if (registration.Length < MIN_REGISTRATION_LENGTH || registration.Length > MAX_REGISTRATION_LENGTH) return false;

            return registration.All(char.IsLetterOrDigit);
        }
    }
}
