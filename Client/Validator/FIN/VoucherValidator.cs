using D69soft.Client.Services.FIN;
using D69soft.Server.Services.HR;
using D69soft.Shared.Models.ViewModels.FIN;
using FluentValidation;

namespace D69soft.Client.Validator.FIN
{
    public class VoucherValidator : AbstractValidator<StockVoucherVM>
    {
        public VoucherValidator()
        {
            When(x => x.VTypeID == "FIN_Input", () =>
            {
                When(x => x.IsTypeUpdate != 2, () =>
                {
                    RuleFor(x => x.VDate).NotEmpty().WithMessage("Không được trống.");

                    RuleFor(x => x.VDesc).NotEmpty().WithMessage("Không được trống.");
                });

                When(x => x.VSubTypeID == "FIN_Input_Purchasing", () =>
                {
                    When(x => x.IsTypeUpdate != 2, () =>
                    {
                        When(x => !x.IsMultipleInvoice, () =>
                        {
                            RuleFor(x => x.VendorCode).NotEmpty().WithMessage("Không được trống.");
                        });
                    });
                });
            });

            When(x => x.VTypeID == "FIN_Output", () =>
            {
                When(x => x.IsTypeUpdate != 2, () =>
                {
                    RuleFor(x => x.VDate).NotEmpty().WithMessage("Không được trống.");

                    RuleFor(x => x.VDesc).NotEmpty().WithMessage("Không được trống.");
                });

                When(x => x.VSubTypeID == "FIN_Output_SalePOS", () =>
                {
                    When(x => x.IsTypeUpdate != 2, () =>
                    {
                        When(x => !x.IsMultipleInvoice, () =>
                        {
                            RuleFor(x => x.CustomerCode).NotEmpty().WithMessage("Không được trống.");
                        });
                    });
                });

            });

            When(x => x.VTypeID == "FIN_Transfer", () =>
            {
                When(x => x.IsTypeUpdate != 2, () =>
                {
                    RuleFor(x => x.VDate).NotEmpty().WithMessage("Không được trống.");

                    RuleFor(x => x.VDesc).NotEmpty().WithMessage("Không được trống.");
                });

                When(x => x.VSubTypeID == "FIN_Transfer_Purchasing", () =>
                {
                    When(x => x.IsTypeUpdate != 2, () =>
                    {
                        When(x => !x.IsMultipleInvoice, () =>
                        {
                            RuleFor(x => x.VendorCode).NotEmpty().WithMessage("Không được trống.");
                        });
                    });
                });

            });

            When(x => x.VTypeID == "FIN_InventoryCheck", () =>
            {
                When(x => x.IsTypeUpdate != 2, () =>
                {
                    RuleFor(x => x.VDate).NotEmpty().WithMessage("Không được trống.");

                    RuleFor(x => x.VDesc).NotEmpty().WithMessage("Không được trống.");
                });
            });

        }
    }

}