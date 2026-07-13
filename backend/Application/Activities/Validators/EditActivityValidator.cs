using FluentValidation;
using Application.Activities.Commands;

namespace Application.Activities.Validators;

public class EditActivityValidator : AbstractValidator<EditActivity.Command>
{
    public EditActivityValidator()
    {
        RuleFor(x => x.ActivityDto.Title).NotEmpty().WithMessage("Title is required");
        RuleFor(x => x.ActivityDto.Description).NotEmpty().WithMessage("Description is required");
        RuleFor(x => x.ActivityDto.Category).NotEmpty().WithMessage("Category is required");
        RuleFor(x => x.ActivityDto.City).NotEmpty().WithMessage("City is required");
        RuleFor(x => x.ActivityDto.Venue).NotEmpty().WithMessage("Venue is required");
        RuleFor(x => x.ActivityDto.Date).NotEmpty().WithMessage("Date is required");
    }
}
