using FluentValidation;
using D69soft.Shared.Models.ViewModels.HR;
using D69soft.Client.Services.HR;

namespace D69soft.Client.Validator.HR
{
    public class DivisionValidator : AbstractValidator<DivisionVM>
    {
        public DivisionValidator(OrganizationalChartService _organizationalChartService)
        {
            RuleFor(x => x.DivisionID).NotEmpty().WithMessage("Không được trống.")
                .Matches(@"^[a-zA-Z0-9]+$").WithMessage("Không hợp lệ.")
                .MinimumLength(2).WithMessage("Tối thiểu 2 kí tự.")
                .Must((x, id) => _organizationalChartService.CheckContainsDivisionID(x.DivisionID).Result).When(x => x.IsTypeUpdate == 0).WithMessage("Đã tồn tại.");

            RuleFor(x => x.DivisionName).NotEmpty().WithMessage("Không được trống.");

            RuleFor(x => x.CodeDivs).NotEmpty().When(x => x.isAutoEserial).WithMessage("Không được trống.")
                .Matches(@"^[a-zA-Z]+$").WithMessage("Chỉ bao gồm ký tự chữ cái không dấu.");
        }
    }

    public class DepartmentValidator : AbstractValidator<DepartmentVM>
    {
        public DepartmentValidator(OrganizationalChartService _organizationalChartService)
        {
            RuleFor(x => x.DepartmentID).NotEmpty().WithMessage("Không được trống.")
                .Matches(@"^[a-zA-Z0-9]+$").WithMessage("Không hợp lệ.")
                .MinimumLength(2).WithMessage("Tối thiểu 2 kí tự.")
                .Must((x, id) => _organizationalChartService.CheckContainsDepartmentID(x.DepartmentID).Result).When(x => x.IsTypeUpdate == 0).WithMessage("Đã tồn tại.");

            RuleFor(x => x.DepartmentName).NotEmpty().WithMessage("Không được trống.");
            RuleFor(x => x.DepartmentGroupID).NotEmpty().WithMessage("Không được trống.");
        }
    }

    public class DepartmentGroupValidator : AbstractValidator<DepartmentGroupVM>
    {
        public DepartmentGroupValidator(OrganizationalChartService _organizationalChartService)
        {
            RuleFor(x => x.DepartmentGroupID).NotEmpty().WithMessage("Không được trống.")
                .Matches(@"^[a-zA-Z0-9]+$").WithMessage("Không hợp lệ.")
                .MinimumLength(2).WithMessage("Tối thiểu 2 kí tự.")
                .Must((x, id) => _organizationalChartService.CheckContainsDepartmentGroupID(x.DepartmentGroupID).Result).When(x => x.IsTypeUpdate == 0).WithMessage("Đã tồn tại.");

            RuleFor(x => x.DepartmentGroupName).NotEmpty().WithMessage("Không được trống.");
        }
    }

    public class SectionValidator : AbstractValidator<SectionVM>
    {
        public SectionValidator(OrganizationalChartService _organizationalChartService)
        {
            RuleFor(x => x.SectionID).NotEmpty().WithMessage("Không được trống.")
            .Matches(@"^[a-zA-Z0-9]+$").WithMessage("Không hợp lệ.")
            .Must((x, id) => _organizationalChartService.CheckContainsSectionID(x.SectionID).Result).When(x => x.IsTypeUpdate == 0).WithMessage("Đã tồn tại.");

            RuleFor(x => x.SectionName).NotEmpty().WithMessage("Không được trống.");
        }
    }

    public class PositionValidator : AbstractValidator<PositionVM>
    {
        public PositionValidator(OrganizationalChartService _organizationalChartService)
        {
            RuleFor(x => x.PositionID).NotEmpty().WithMessage("Không được trống.")
                .Matches(@"^[a-zA-Z0-9]+$").WithMessage("Không hợp lệ.")
                .MinimumLength(2).WithMessage("Tối thiểu 2 kí tự.")
                .Must((x, id) => _organizationalChartService.CheckContainsPositionID(x.PositionID).Result).When(x => x.IsTypeUpdate == 0).WithMessage("Đã tồn tại.");

            RuleFor(x => x.PositionName).NotEmpty().WithMessage("Không được trống.");
            RuleFor(x => x.PositionGroupID).NotEmpty().WithMessage("Không được trống.");
        }
    }

    public class PositionGroupValidator : AbstractValidator<PositionGroupVM>
    {
        public PositionGroupValidator(OrganizationalChartService _organizationalChartService)
        {
            RuleFor(x => x.PositionGroupID).NotEmpty().WithMessage("Không được trống.")
                .Matches(@"^[a-zA-Z0-9]+$").WithMessage("Không hợp lệ.")
                .MinimumLength(2).WithMessage("Tối thiểu 2 kí tự.")
                .Must((x, id) => _organizationalChartService.CheckContainsPositionGroupID(x.PositionGroupID).Result).When(x => x.IsTypeUpdate == 0).WithMessage("Đã tồn tại.");

            RuleFor(x => x.PositionGroupName).NotEmpty().WithMessage("Không được trống.");
        }
    }
}