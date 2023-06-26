using D69soft.Client.Services.FIN;
using D69soft.Server.Services.HR;
using D69soft.Shared.Models.ViewModels.FIN;
using FluentValidation;

namespace D69soft.Client.Validator.FIN
{
    public class VendorValidator : AbstractValidator<VendorVM>
    {
        public VendorValidator()
        {
            RuleFor(x => x.VendorName).NotEmpty().WithMessage("Không được trống.");
        }
    }

}