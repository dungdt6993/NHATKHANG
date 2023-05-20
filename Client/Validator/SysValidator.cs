using FluentValidation;
using D69soft.Shared.Models.ViewModels.SYSTEM;

namespace D69soft.Client.Validator
{
    public class RptValidator : AbstractValidator<RptVM>
    {
        public RptValidator()
        {
            RuleFor(x => x.RptGrpID).NotEmpty().WithMessage("Nhóm báo cáo không được trống");
            RuleFor(x => x.RptName).NotEmpty().WithMessage("Tên báo cáo không được trống");
            RuleFor(x => x.RptUrl).NotEmpty().WithMessage("File báo cáo không được trống");
        }
    }

    public class RptGrpValidator : AbstractValidator<RptGrpVM>
    {
        public RptGrpValidator()
        {
            RuleFor(x => x.RptGrpName).NotEmpty().WithMessage("Tên nhóm báo cáo không được trống");
        }
    }
}
