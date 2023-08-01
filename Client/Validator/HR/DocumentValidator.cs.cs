using D69soft.Shared.Models.ViewModels.HR;
using FluentValidation;

namespace D69soft.Client.Validator.HR
{
    public class DocumentValidator : AbstractValidator<DocumentVM>
    {
        public DocumentValidator()
        {
            When(x => x.GroupType == "DOCForm", () =>
            {
                RuleFor(x => x.DocName).NotEmpty().WithMessage("Không được trống.");
            });

            When(x => x.GroupType == "DOCBoat", () =>
            {
                RuleFor(x => x.DepartmentID).NotEmpty().WithMessage("Không được trống.");

                RuleFor(x => x.DateOfIssue).NotEmpty().When(x => x.ExpDate != null).WithMessage("Không được trống.");

                RuleFor(x => x.ExpDate).Must((x, ExpDate) => ExpDate > x.DateOfIssue).When(x => x.ExpDate != null).WithMessage("Ngày hết hạn phải lớn hơn ngày cấp.");
            });

            When(x => x.GroupType == "DOCStaff", () =>
            {
                RuleFor(x => x.Eserial).NotEmpty().WithMessage("Không được trống.");

                RuleFor(x => x.DateOfIssue).NotEmpty().When(x => x.ExpDate != null).WithMessage("Không được trống.");

                RuleFor(x => x.ExpDate).Must((x, ExpDate) => ExpDate > x.DateOfIssue).When(x => x.ExpDate != null).WithMessage("Ngày hết hạn phải lớn hơn ngày cấp.");
            });

            When(x => x.GroupType == "DOCLegal", () =>
            {
                RuleFor(x => x.DateOfIssue).NotEmpty().When(x => x.ExpDate != null).WithMessage("Không được trống.");

                RuleFor(x => x.ExpDate).Must((x, ExpDate) => ExpDate > x.DateOfIssue).When(x => x.ExpDate != null).WithMessage("Ngày hết hạn phải lớn hơn ngày cấp.");
            });


            RuleFor(x => x.DocTypeID).NotEmpty().WithMessage("Không được trống.");


        }
    }

    public class DocumentTypeValidator : AbstractValidator<DocumentTypeVM>
    {
        public DocumentTypeValidator()
        {
            RuleFor(x => x.DocTypeName).NotEmpty().WithMessage("Không được trống.");
        }
    }

}