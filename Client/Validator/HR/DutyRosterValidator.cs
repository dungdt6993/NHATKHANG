using D69soft.Client.Services.FIN;
using D69soft.Client.Services.HR;
using D69soft.Shared.Models.ViewModels.HR;
using FluentValidation;

namespace D69soft.Client.Validator.HR
{
    public class LockDutyRosterValidator : AbstractValidator<LockDutyRosterVM>
    {
        public LockDutyRosterValidator()
        {
            RuleFor(x => x.LockTo).NotEmpty().When(x => x.IsTypeUpdate == 1).WithMessage("Không được trống.");
        }
    }

    public class ShiftValidator : AbstractValidator<ShiftVM>
    {
        public ShiftValidator(DutyRosterService _dutyRosterService)
        {
            When(x => x.IsTypeUpdate != 2, () =>
            {
                RuleFor(x => x.ShiftID).NotEmpty().WithMessage("Không được trống.")
                .Matches(@"^[a-zA-Z0-9]+$").WithMessage("Không hợp lệ.")
                .MinimumLength(2).WithMessage("Tối thiểu 2 kí tự.")
                .MustAsync(async (id, cancellation) =>
                {
                    bool result = true;
                    if (!String.IsNullOrEmpty(id))
                    {
                        result = await _dutyRosterService.ContainsShiftID(id);
                    }
                    return result;
                }).When(x => x.IsTypeUpdate == 0).WithMessage("Đã tồn tại.");

                RuleFor(x => x.ShiftName).NotEmpty().WithMessage("Không được trống.");
                RuleFor(x => x.ShiftTypeID).NotEmpty().WithMessage("Không được trống.");

                RuleFor(x => x.BeginTime).NotEmpty().When(x => x.EndTime != null).WithMessage("Không được trống.");
                RuleFor(x => x.EndTime).NotEmpty().When(x => x.BeginTime != null).WithMessage("Không được trống.");

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