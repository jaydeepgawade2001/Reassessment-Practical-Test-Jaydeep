using FluentValidation;
using ReassessmentApp.Application.DTOs;
using System;

namespace ReassessmentApp.Application.Validators
{
    public class CreateBookingDtoValidator : AbstractValidator<CreateBookingDto>
    {
        public CreateBookingDtoValidator()
        {
            RuleFor(x => x.RoomId)
                .NotEmpty().WithMessage("RoomId is required");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

            RuleFor(x => x.StartTime)
                .NotEmpty().WithMessage("StartTime is required")
                .GreaterThan(DateTime.UtcNow).WithMessage("StartTime must be in the future");

            RuleFor(x => x.EndTime)
                .NotEmpty().WithMessage("EndTime is required")
                .GreaterThan(x => x.StartTime).WithMessage("EndTime must be after StartTime");

            RuleFor(x => x.CreatedBy)
                .NotEmpty().WithMessage("CreatedBy is required");
        }
    }
}
