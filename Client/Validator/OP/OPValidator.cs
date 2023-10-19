using D69soft.Client.Services.OP;
using D69soft.Shared.Models.ViewModels.OP;
using FluentValidation;
using Model.ViewModels.OP;

namespace D69soft.Client.Validator.EA
{
    public class OPValidator : AbstractValidator<VehicleVM>
    {
        public OPValidator(OPService _opService)
        {
            RuleFor(x => x.VehicleCode).NotEmpty().WithMessage("Không được trống.")
            .Matches(@"^[a-zA-Z0-9]+$").WithMessage("Không hợp lệ.")
            .MinimumLength(2).WithMessage("Tối thiểu 2 kí tự.")
            .MustAsync(async (id, cancellation) =>
            {
                bool result = true;
                if (!String.IsNullOrEmpty(id))
                {
                    result = await _opService.CheckContainsVehicleCode(id);
                }
                return result;
            }).When(x => x.IsTypeUpdate == 0).WithMessage("Đã tồn tại.");

            RuleFor(x => x.VehicleName).NotEmpty().WithMessage("Không được trống.");
            RuleFor(x => x.DepartmentID).NotEmpty().WithMessage("Không được trống.");
        }
    }

}