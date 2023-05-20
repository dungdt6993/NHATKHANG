using D69soft.Shared.Models.ViewModels.EA;
using FluentValidation;

namespace D69soft.Client.Validator.EA
{
    public class RequestValidator : AbstractValidator<RequestVM>
    {
        public RequestValidator()
        {
            RuleFor(x => x.DeptRequest).NotEmpty().WithMessage("Không được trống.");
            RuleFor(x => x.ReasonOfRequest).NotEmpty().WithMessage("Không được trống.");
        }
    }

}