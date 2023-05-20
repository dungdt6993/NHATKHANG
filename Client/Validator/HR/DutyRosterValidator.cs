using D69soft.Client.Services.HR;
using D69soft.Shared.Models.ViewModels.HR;
using FluentValidation;

namespace D69soft.Client.Validator.HR
{
    public class LockDutyRosterValidator : AbstractValidator<LockDutyRosterVM>
    {
        public LockDutyRosterValidator()
        {
            RuleFor(x => x.LockTo).NotEmpty().When(x => x.IsTypeUpdate == 1).WithMessage("Ngày khóa không được trống.");
        }
    }

    public class ShiftValidator : AbstractValidator<ShiftVM>
    {
        public ShiftValidator(DutyRosterService _dutyRosterService)
        {
            When(x => x.IsTypeUpdate != 2, () =>
            {
                RuleFor(x => x.ShiftID).NotEmpty().WithMessage("Mã ca không được trống.")
                .Matches(@"^[a-zA-Z0-9]+$").WithMessage("Mã ca không hợp lệ.")
                .MinimumLength(2).WithMessage("Tối thiểu 2 kí tự.")
                .Must((x, id) => _dutyRosterService.ContainsShiftID(x.ShiftID).Result).When(x => x.IsTypeUpdate == 0).WithMessage("Mã ca đã tồn tại.");

                RuleFor(x => x.ShiftName).NotEmpty().WithMessage("Tên ca không được trống.");
                RuleFor(x => x.ShiftTypeID).NotEmpty().WithMessage("Loại ca không được trống.");

                RuleFor(x => x.BeginTime).NotEmpty().When(x => x.EndTime != null).WithMessage("Giờ vào không được trống.");
                RuleFor(x => x.EndTime).NotEmpty().When(x => x.BeginTime != null).WithMessage("Giờ ra không được trống.");

                RuleFor(x => x.EndTime).Must((x, EndTime) => EndTime != x.BeginTime).When(x => x.EndTime != null).WithMessage("Giờ ra phải khác giờ vào.");

            });
        }
    }

    public class WorkPlanValidator : AbstractValidator<DutyRosterVM>
    {
        public WorkPlanValidator()
        {
            RuleFor(x => x.WorkPlanName).NotEmpty().WithMessage("Không được trống.");

            RuleFor(x => x.WorkPlanStartDate).NotEmpty().WithMessage("Không được trống.");

            RuleFor(x => x.WorkPlanDeadline).Must((x, WorkPlanDeadline) => WorkPlanDeadline >= x.WorkPlanStartDate).When(x => x.WorkPlanDeadline != null).WithMessage("Thời hạn phải lớn hơn hoặc bằng ngày bắt đầu.");
        }
    }

}