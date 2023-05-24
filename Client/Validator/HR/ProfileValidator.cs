using D69soft.Server.Services.HR;
using D69soft.Shared.Models.ViewModels.HR;
using FluentValidation;

namespace D69soft.Client.Validator.HR
{
    public class ProfileValidator : AbstractValidator<ProfileVM>
    {
        private readonly ProfileService _profileService;

        public ProfileValidator(ProfileService profileService)
        {
            _profileService = profileService;

            RuleFor(x => x.Eserial).NotEmpty().WithMessage("Mã nhân viên không được trống.").When(x => x.isAutoEserial == false)
            .Matches(@"^[A-Za-z0-9_.]+$").WithMessage("Mã nhân viên không hợp lệ.")
            .MaximumLength(20).WithMessage("Mã nhân viên tối đa 20 ký tự.")

            .MustAsync(async (eserial, cancellation) =>
            {
                bool result = await _profileService.CheckContainsEserial(eserial);
                return result;
            }).When(x => x.IsTypeUpdate == 0).WithMessage("Mã nhân viên đã tồn tại.");

            RuleFor(x => x.LastName).NotEmpty().WithMessage("Họ không được trống.");

            RuleFor(x => x.FirstName).NotEmpty().WithMessage("Tên không được trống.");

            RuleFor(x => x.Birthday).NotEmpty().WithMessage("Ngày sinh không được trống.");

            RuleFor(x => x.IDCard).NotEmpty().WithMessage("Căn cước/Hộ chiếu không được trống.");

            RuleFor(x => x.JoinDate).NotEmpty().WithMessage("Ngày vào làm không được trống.");

            RuleFor(x => x.StartContractDate).NotEmpty().WithMessage("Ngày ký hợp đồng không được trống.");
            RuleFor(x => x.StartContractDate).Must((x, StartContractDate) => StartContractDate >= x.JoinDate).WithMessage("Ngày ký HĐ phải lớn hơn hoặc bằng ngày vào làm.");
            RuleFor(x => x.StartContractDate).Must((x, StartContractDate) => StartContractDate > x.Old_StartContractDate).When(x => x.ckContractExtension == 2).WithMessage("Ngày ký HĐ mới phải lớn hơn ngày ký HĐ hiện tại.");

            RuleFor(x => x.JobStartDate).NotEmpty().WithMessage("Ngày bắt đầu công việc không được trống.");
            RuleFor(x => x.JobStartDate).Must((x, JobStartDate) => JobStartDate >= x.JoinDate).WithMessage("Ngày bắt đầu công việc phải lớn hơn hoặc bằng ngày vào làm.");
            RuleFor(x => x.JobStartDate).Must((x, JobStartDate) => JobStartDate < x.EndContractDate).When(x => x.EndContractDate != null).WithMessage("Ngày bắt đầu công việc phải nhỏ hơn ngày hết hạn hợp đồng.");
            RuleFor(x => x.JobStartDate).Must((x, JobStartDate) => JobStartDate > x.Old_JobStartDate).When(x => x.ckContractExtension == 0).When(x => x.ckJob != 0).WithMessage("Ngày làm việc mới phải lớn hơn ngày làm việc hiện tại.");

            RuleFor(x => x.EndContractDate).Must((x, EndContractDate) => EndContractDate > x.StartContractDate).WithMessage("Ngày hết hạn HĐ phải lớn hơn ngày ký HĐ.")
                .Must((x, EndContractDate) => EndContractDate > x.JobStartDate).When(x => x.EndContractDate != null).WithMessage("Ngày hết hạn HĐ phải lớn hơn ngày bắt đầu công việc.");

            RuleFor(x => x.DepartmentID).NotEmpty().WithMessage("Bộ phận không được trống.");

            RuleFor(x => x.SectionID).NotEmpty().WithMessage("Khu vực không được trống.");

            RuleFor(x => x.PositionID).NotEmpty().WithMessage("Vị trí không được trống.");

            RuleFor(x => x.ContractTypeID).NotEmpty().WithMessage("Loại hợp đồng không được trống.");

            RuleFor(x => x.WorkTypeID).NotEmpty().WithMessage("Loại công việc không được trống.");

            RuleFor(x => x.BankCode).NotEmpty().When(x => (x.BankAccount ?? "") != string.Empty).WithMessage("Mã ngân hàng không được trống.");

            RuleFor(x => x.BankName).NotEmpty().When(x => (x.BankAccount ?? "") != string.Empty).WithMessage("Tên ngân hàng không được trống.");

            RuleFor(x => x.BankAccount).NotEmpty().When(x => x.SalaryByBank == 1).WithMessage("Hình thức trả lương chuyển khoản nên TK ngân hàng không được trống.");


            RuleFor(x => x.BeginSalaryDate).NotEmpty().WithMessage("Ngày tính lương không được trống.").When(x => x.TotalSalary > 0);
            RuleFor(x => x.TotalSalary).NotEmpty().WithMessage("Tổng lương phải lớn hơn 0.").When(x => x.BeginSalaryDate != null);


            When(x => x.ckSal != 0, () =>
            {
                RuleFor(x => x.BeginSalaryDate)
                .Must((x, BeginSalaryDate) => BeginSalaryDate > x.StartContractDate).WithMessage("Ngày tính lương phải lớn hơn hoặc bằng ngày ký HĐ.")
                .Must((x, BeginSalaryDate) => BeginSalaryDate > x.Old_BeginSalaryDate).WithMessage("Ngày tính lương mới phải lớn hơn ngày tính lương hiện tại.")
                .When(x => x.ckContractExtension == 0);

                RuleFor(x => x.Reason).NotEmpty().WithMessage("Ghi chú điều chỉnh lương không được trống.");
            });

            When(x => x.IsTypeUpdate == 4, () =>
            {
                RuleFor(x => x.TerminateDate).NotEmpty().WithMessage("Ngày chấm dứt hợp đồng không được trống.");

                RuleFor(x => x.ReasonTerminate).NotEmpty().WithMessage("Lý do chấm dứt hợp đồng không được trống.");
            });

        }
    }

    //public class EditEserialValidator : AbstractValidator<EditEserialVM>
    //{
    //    public EditEserialValidator(ProfileService _profileService)
    //    {
    //        RuleFor(x => x.NewEserial).NotEmpty().WithMessage("Mã nhân viên mới không được trống.")
    //        .Matches(@"^[A-Za-z0-9_.]+$").WithMessage("Mã nhân viên không hợp lệ.")
    //        .MaximumLength(20).WithMessage("Mã nhân viên tối đa 20 ký tự.")
    //        .Must((x, eserial) => _profileService.CheckContainsEserial(x.Eserial).Result).WithMessage("Mã nhân viên đã tồn tại.");

    //    }
    //}

    //public class ContractTypeValidator : AbstractValidator<ContractTypeVM>
    //{
    //    public ContractTypeValidator(ProfileService _profileService)
    //    {
    //        When(x => x.IsTypeUpdate != 2, () =>
    //          {
    //              RuleFor(x => x.ContractTypeID).NotEmpty().WithMessage("Mã loại hợp đồng không được trống.")
    //              .Matches(@"^[a-zA-Z0-9]+$").WithMessage("Mã loại hợp đồng không hợp lệ.")
    //              .MinimumLength(2).WithMessage("Tối thiểu 2 kí tự.")
    //              .Must((x, id) => _profileService.ContainsContractTypeID(x.ContractTypeGroupID).Result).When(x => x.IsTypeUpdate == 0).WithMessage("Mã loại hợp đồng đã tồn tại.");

    //              RuleFor(x => x.ContractTypeName).NotEmpty().WithMessage("Tên loại hợp đồng không được trống.");
    //              RuleFor(x => x.ContractTypeGroupID).NotEmpty().WithMessage("Nhóm hợp đồng không được trống.");
    //          });
    //    }
    //}

    //public class WorkTypeValidator : AbstractValidator<WorkTypeVM>
    //{
    //    public WorkTypeValidator(ProfileService _profileService)
    //    {
    //        When(x => x.IsTypeUpdate != 2, () =>
    //        {
    //            RuleFor(x => x.WorkTypeID).NotEmpty().WithMessage("Mã loại công việc không được trống.")
    //            .Matches(@"^[a-zA-Z0-9]+$").WithMessage("Mã loại công việc không hợp lệ.")
    //            .MinimumLength(2).WithMessage("Tối thiểu 2 kí tự.")
    //            .Must((x, id) => _profileService.ContainsWorkTypeID(x.WorkTypeID).Result).When(x => x.IsTypeUpdate == 0).WithMessage("Mã loại công việc đã tồn tại.");

    //            RuleFor(x => x.WorkTypeName).NotEmpty().WithMessage("Tên loại công việc không được trống.");
    //        });
    //    }
    //}

    //public class ProfileRelationshipValidator : AbstractValidator<ProfileRelationshipVM>
    //{
    //    public ProfileRelationshipValidator()
    //    {
    //        When(x => x.IsTypeUpdate != 2, () =>
    //        {
    //            RuleFor(x => x.Rela_FullName).NotEmpty().WithMessage("Họ tên không được trống.");
    //            RuleFor(x => x.Rela_Birthday).NotEmpty().WithMessage("Ngày sinh không được trống.");
    //            RuleFor(x => x.Rela_ValidTo).NotEmpty().When(x=>x.isEmployeeTax).WithMessage("Hiệu lực không được trống.");
    //        });
    //    }
    //}

}
