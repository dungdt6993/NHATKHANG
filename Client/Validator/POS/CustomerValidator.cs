using D69soft.Client.Services.CRM;
using D69soft.Shared.Models.ViewModels.CRM;
using FluentValidation;

namespace D69soft.Client.Validator.POS
{
    public class CustomerValidator : AbstractValidator<CustomerVM>
    {
        public CustomerValidator(CustomerService _customerService)
        {
            RuleFor(x => x.CustomerID).Matches(@"^[a-zA-Z0-9]+$").WithMessage("Mã khách hàng không hợp lệ.")
            .Must((x, id) => _customerService.CheckContains_Customer(x.CustomerID).Result).When(x => x.IsTypeUpdate == 0).WithMessage("Mã khách hàng đã tồn tại.");

            RuleFor(x => x.CustomerName).NotEmpty().WithMessage("Tên khách hàng không được trống.");

            RuleFor(x => x.Tel).NotEmpty().WithMessage("Số điện thoại không được trống.")
                .Matches(@"^[0-9]+$").WithMessage("Định dạng số (Ví dụ:0912345678).")
                .Must((x, id) => _customerService.CheckContains_Tel(x.Tel).Result).When(x => x.IsTypeUpdate == 0).WithMessage("Số điện thoại đã tồn tại.");
        }
    }

}
