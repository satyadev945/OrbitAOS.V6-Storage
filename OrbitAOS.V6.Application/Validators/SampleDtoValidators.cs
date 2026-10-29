using FluentValidation;
using OrbitAOS.V6.Application.DTOs;

namespace OrbitAOS.V6.Application.Validators
{
    /// <summary>
    /// Validator for CreateSampleDto
    /// </summary>
    public class CreateSampleDtoValidator : AbstractValidator<CreateSampleDto>
    {
        public CreateSampleDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters");
        }
    }

    /// <summary>
    /// Validator for UpdateSampleDto
    /// </summary>
    public class UpdateSampleDtoValidator : AbstractValidator<UpdateSampleDto>
    {
        public UpdateSampleDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters");
        }
    }
}
