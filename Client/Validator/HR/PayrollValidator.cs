using FluentValidation;
using D69soft.Shared.Models.ViewModels.HR;
using D69soft.Client.Services.HR;

namespace D69soft.Client.Validator.HR
{
    public class MonthlyIncomeTrnOtherValidator : AbstractValidator<MonthlyIncomeTrnOtherVM>
    {
        public MonthlyIncomeTrnOtherValidator()
        {
            When(x => x.IsTypeUpdate != 3, () =>
            {
                RuleFor(x => x.UnitPrice).NotEmpty().WithMessage("Đơn giá phải khác 0.");
                RuleFor(x => x.Qty).NotEmpty().WithMessage("Hệ số phải khác 0.");
            });
            RuleFor(x => x.Note).NotEmpty().WithMessage("Ghi chú không được trống.");
        }
    }

    public class SalaryDefValidator : AbstractValidator<SalaryDefVM>
    {
        public SalaryDefValidator()
        {
            RuleFor(x => x.SalaryTypeName).NotEmpty().WithMessage("Diễn giải không được trống.");
            When(x => !x.isCalcByShift && x.isUpdate, () =>
            {
                RuleFor(x => x.TrnCode).NotEmpty().WithMessage("Giao dịch không được trống.");
                RuleFor(x => x.TrnSubCode).NotEmpty().WithMessage("Giao dịch không được trống.");
            });
        }
    }

    public class SalTrnCodeValidator : AbstractValidator<SalaryTransactionCodeVM>
    {
        public SalTrnCodeValidator(PayrollService _payrollService)
        {
            When(x => x.IsTypeUpdate != 2, () =>
            {
                RuleFor(x => x.TrnGroupCode).NotEmpty().WithMessage("Chưa chọn nhóm giao dịch.");

                RuleFor(x => x.TrnSubCode).NotEmpty().WithMessage("Mã giao dịch không được trống.")
                .Must((x, TrnCode) => _payrollService.ContainsTrnCodeID(x.TrnGroupCode, x.TrnSubCode).Result).When(x => x.IsTypeUpdate == 0).WithMessage("Mã giao dịch đã tồn tại.");

                RuleFor(x => x.TrnName).NotEmpty().WithMessage("Tên giao dịch không được trống.");
            });
        }
    }

}