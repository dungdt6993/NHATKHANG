using D69soft.Client.Services.HR;
using D69soft.Shared.Models.Entities.HR;
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
                RuleFor(x => x.ALNoteAddBalance).NotEmpty().WithMessage("Không được trống.");
            });

            When(x => x.dayOffType == "DO" && x.CLDOAddBalance != 0, () =>
            {
                RuleFor(x => x.CLDONoteAddBalance).NotEmpty().WithMessage("Không được trống.");
            });

            When(x => x.dayOffType == "PH" && x.CLPHAddBalance != 0, () =>
            {
                RuleFor(x => x.CLPHNoteAddBalance).NotEmpty().WithMessage("Không được trống.");
            });

        }
    }

    public class PublicHolidayValidator : AbstractValidator<PublicHolidayDefVM>
    {
        public PublicHolidayValidator(DayOffService _dayOffService)
        {
            When(x => x.IsTypeUpdate != 2, () =>
            {
                RuleFor(x => x.PHName).NotEmpty().WithMessage("Không được trống.");

                RuleFor(x => x.PHMonth).NotEmpty().WithMessage("Không được trống.");

                //RuleFor(x => x.PHDay).NotEmpty().WithMessage("Không được trống.")
                //    .MustAsync(async (PHDay, cancellation) =>
                //    {
                //        bool result = true;
                //        if (PHDay != 0)
                //        {
                //            result = await _dayOffService.ContainsPublicHoliday(PHDay, PHMonth, _isLunar);
                //        }
                //        return result;
                //    }).When(x => x.IsTypeUpdate == 0).WithMessage("Đã tồn tại.");
            });
        }
    }

}