using D69soft.Client.Services.CRM;
using D69soft.Client.Services.HR;
using D69soft.Shared.Models.ViewModels.CRM;
using FluentValidation;

namespace D69soft.Client.Validator.CRM
{
    public class CustomerValidator : AbstractValidator<CustomerVM>
    {
        public CustomerValidator(CustomerService _customerService)
        {
            RuleFor(x => x.CustomerName).NotEmpty().WithMessage("Không được trống.");

            RuleFor(x => x.CustomerTel).NotEmpty().WithMessage("Không được trống.")
                .Matches(@"^[0-9]+$").WithMessage("Định dạng số (Ví dụ:0912345678).")
                .MustAsync(async (id, cancellation) =>
                    {
                        bool result = true;
                        if (!String.IsNullOrEmpty(id))
                        {
                            result = await _customerService.ContainsCustomerTel(id);
                        }
                        return result;
                    }).When(x => x.IsTypeUpdate == 0).WithMessage("Đã tồn tại.");
        }
    }

}
