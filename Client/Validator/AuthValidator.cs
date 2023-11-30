using FluentValidation;
using D69soft.Shared.Models.ViewModels.SYSTEM;

namespace D69soft.Client.Validator
{
    public class LoginValidator : AbstractValidator<UserVM>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Eserial).NotEmpty().WithMessage("Không được trống.");

            RuleFor(x => x.User_Password).NotEmpty().WithMessage("Không được trống.").MinimumLength(6).WithMessage("Tối thiểu 6 kí tự.");
        }
    }

    public class ChangePassValidator : AbstractValidator<ChangePassVM>
    {
        public ChangePassValidator()
        {
            RuleFor(x => x.NewPass).NotEmpty().WithMessage("Không được trống").MinimumLength(6).WithMessage("Tối thiểu 6 ký tự");

            RuleFor(x => x.ComfirmNewPass).NotEmpty().WithMessage("Không được trống").MinimumLength(6).WithMessage("Tối thiểu 6 ký tự").Must((x, ComfirmNewPass) => ComfirmNewPass == x.NewPass).WithMessage("Mật khẩu và xác nhận mật khẩu không trùng nhau.");
        }
    }
}