using D69soft.Client.Services.FIN;
using D69soft.Server.Services.HR;
using D69soft.Shared.Models.ViewModels.FIN;
using FluentValidation;

namespace D69soft.Client.Validator.FIN
{
    public class VoucherValidator : AbstractValidator<VoucherVM>
    {
        public VoucherValidator()
        {
            When(x => x.VTypeID == "FIN_Purchasing", () =>
            {
                When(x => x.IsTypeUpdate != 2, () =>
                {
                    RuleFor(x => x.VDate).NotEmpty().WithMessage("Không được trống.");

                    RuleFor(x => x.VDesc).NotEmpty().WithMessage("Không được trống.");

                    RuleFor(x => x.VendorCode).NotEmpty().WithMessage("Không được trống.");
                });

                When(x => x.IsTypeUpdate == 6 && x.IsInvoice, () =>
                {
                    RuleFor(x => x.InvoiceNumber).NotEmpty().WithMessage("Không được trống.");
                    RuleFor(x => x.InvoiceDate).NotEmpty().WithMessage("Không được trống.");
                });
            });

            When(x => x.VTypeID == "FIN_Sale", () =>
            {
                When(x => x.IsTypeUpdate != 2, () =>
                {
                    RuleFor(x => x.VDate).NotEmpty().WithMessage("Không được trống.");

                    RuleFor(x => x.VDesc).NotEmpty().WithMessage("Không được trống.");

                    RuleFor(x => x.CustomerCode).NotEmpty().WithMessage("Không được trống.");
                });

                When(x => x.IsTypeUpdate == 6 && x.IsInvoice, () =>
                {
                    RuleFor(x => x.InvoiceNumber).NotEmpty().WithMessage("Không được trống.");
                    RuleFor(x => x.InvoiceDate).NotEmpty().WithMessage("Không được trống.");
                });
            });

            When(x => x.VTypeID == "FIN_Input", () =>
            {
                When(x => x.IsTypeUpdate != 2, () =>
                {
                    RuleFor(x => x.VDate).NotEmpty().WithMessage("Không được trống.");

                    RuleFor(x => x.VDesc).NotEmpty().WithMessage("Không được trống.");
                });
            });

            When(x => x.VTypeID == "FIN_Output", () =>
            {
                When(x => x.IsTypeUpdate != 2, () =>
                {
                    RuleFor(x => x.VDate).NotEmpty().WithMessage("Không được trống.");

                    RuleFor(x => x.VDesc).NotEmpty().WithMessage("Không được trống.");
                });
            });

            When(x => x.VTypeID == "FIN_Trf", () =>
            {
                When(x => x.IsTypeUpdate != 2, () =>
                {
                    RuleFor(x => x.VDate).NotEmpty().WithMessage("Không được trống.");

                    RuleFor(x => x.VDesc).NotEmpty().WithMessage("Không được trống.");
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

            When(x => x.VTypeID == "FIN_Cash_Payment" || x.VTypeID == "FIN_Cash_Receipt" || x.VTypeID == "FIN_Deposit_Credit" || x.VTypeID == "FIN_Deposit_Debit", () =>
            {
                When(x => x.IsTypeUpdate != 2, () =>
                {
                    RuleFor(x => x.VSubTypeID).NotEmpty().WithMessage("Không được trống.");

                    RuleFor(x => x.VDate).NotEmpty().WithMessage("Không được trống.");

                    RuleFor(x => x.VDesc).NotEmpty().WithMessage("Không được trống.");
                });
            });

            When(x => x.PaymentTypeCode == "BANK" || x.VTypeID == "FIN_Deposit_Credit" || x.VTypeID == "FIN_Deposit_Debit", () =>
            {
                When(x => x.IsTypeUpdate != 2, () =>
                {
                    RuleFor(x => x.BankAccountID).NotEmpty().WithMessage("Không được trống.");
                });
            });

        }
    }

}