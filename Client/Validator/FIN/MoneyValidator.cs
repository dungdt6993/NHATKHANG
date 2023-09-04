using D69soft.Client.Services.FIN;
using D69soft.Client.Services.HR;
using D69soft.Shared.Models.ViewModels.FIN;
using FluentValidation;

namespace D69soft.Client.Validator.FIN
{
    public class BankValidator : AbstractValidator<BankVM>
    {
        public BankValidator(MoneyService _moneyService)
        {
            RuleFor(x => x.BankShortName).NotEmpty().WithMessage("Không được trống.");
            RuleFor(x => x.BankFullName).NotEmpty().WithMessage("Không được trống.");

            RuleFor(x => x.SwiftCode).NotEmpty().WithMessage("Không được trống.")
                .Matches(@"^[a-zA-Z0-9]+$").WithMessage("Không hợp lệ.")
                .MustAsync(async (id, cancellation) =>
                {
                    bool result = true;
                    if (!String.IsNullOrEmpty(id))
                    {
                        result = await _moneyService.CheckContainsSwiftCode(id);
                    }
                    return result;
                }).When(x => x.IsTypeUpdate == 0).WithMessage("Đã tồn tại.");
        }
    }

    public class BankAccountValidator : AbstractValidator<BankAccountVM>
    {
        public BankAccountValidator(MoneyService _moneyService)
        {
            RuleFor(x => x.SwiftCode).NotEmpty().WithMessage("Không được trống.");
            RuleFor(x => x.AccountHolder).NotEmpty().WithMessage("Không được trống.");

            RuleFor(x => x.BankAccount).NotEmpty().WithMessage("Không được trống.");
                //.Matches(@"^[a-zA-Z0-9]+$").WithMessage("Không hợp lệ.")
                //.MustAsync(async (id, cancellation) =>
                //{
                //    bool result = true;
                //    if (!String.IsNullOrEmpty(id))
                //    {
                //        result = await _moneyService.CheckContainsSwiftCode(id);
                //    }
                //    return result;
                //}).When(x => x.IsTypeUpdate == 0).WithMessage("Đã tồn tại.");
        }
    }

}
