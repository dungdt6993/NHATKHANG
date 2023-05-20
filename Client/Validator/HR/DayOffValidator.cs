using D69soft.Client.Services.HR;
using D69soft.Shared.Models.ViewModels.HR;
using FluentValidation;

namespace D69soft.Client.Validator.HR
{
    public class DayOffValidator : AbstractValidator<DayOffVM>
    {
        public DayOffValidator()
        {

            When(x => x.dayOffType == "AL" && x.ALAddBalance != 0, () =>
            {
                RuleFor(x => x.ALNoteAddBalance).NotEmpty().WithMessage("Lý do không được trống.");
            });

            When(x => x.dayOffType == "DO" && x.CLDOAddBalance != 0, () =>
            {
                RuleFor(x => x.CLDONoteAddBalance).NotEmpty().WithMessage("Lý do không được trống.");
            });

            When(x => x.dayOffType == "PH" && x.CLPHAddBalance != 0, () =>
            {
                RuleFor(x => x.CLPHNoteAddBalance).NotEmpty().WithMessage("Lý do không được trống.");
            });

        }
    }

    public class PublicHolidayValidator : AbstractValidator<PublicHolidayDefVM>
    {
        public PublicHolidayValidator(DayOffService _dayOffService)
        {
            When(x => x.IsTypeUpdate != 2, () =>
            {
                RuleFor(x => x.PHName).NotEmpty().WithMessage("Tên ngày lễ tết không được trống.");

                RuleFor(x => x.PHMonth).NotEmpty().WithMessage("Tháng không được trống.");

                RuleFor(x => x.PHDay).NotEmpty().WithMessage("Ngày không được trống.")
                .Must((x, PHDay) => _dayOffService.ContainsPublicHoliday(x.PHDay, x.PHMonth, x.isLunar).Result).When(x=>x.IsTypeUpdate==0).WithMessage("Ngày lễ tết đã tồn tại.");

            });
        }
    }

}