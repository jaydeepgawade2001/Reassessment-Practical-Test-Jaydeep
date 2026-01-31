using FluentValidation;
using ReassessmentApp.Application.DTOs;

namespace ReassessmentApp.Application.Validators
{
    public class CreateRoomDtoValidator : AbstractValidator<CreateRoomDto>
    {
        public CreateRoomDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");

            RuleFor(x => x.Capacity)
                .GreaterThan(0).WithMessage("Capacity must be at least 1");

            RuleFor(x => x.Location)
                .NotEmpty().WithMessage("Location is required");
        }
    }
}
