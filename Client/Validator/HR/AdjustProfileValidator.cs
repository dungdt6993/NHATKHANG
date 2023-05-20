using D69soft.Shared.Models.ViewModels.HR;
using FluentValidation;

namespace D69soft.Client.Validator.HR
{
    public class AdjustProfileValidator : AbstractValidator<AdjustProfileVM>
    {
        public AdjustProfileValidator()
        {
            RuleFor(x => x.AdjustProfileName).NotEmpty().WithMessage("Diễn giải không được trống.");
        }
    }
}