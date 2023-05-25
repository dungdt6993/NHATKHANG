using D69soft.Client.Services.FIN;
using D69soft.Server.Services.HR;
using D69soft.Shared.Models.ViewModels.FIN;
using FluentValidation;

namespace D69soft.Client.Validator.HR
{
    public class ItemValidator : AbstractValidator<ItemsVM>
    {
        public ItemValidator(InventoryService _inventoryService)
        {
            When(x => x.IsTypeUpdate != 2, () =>
            {
                RuleFor(x => x.ICode)
                .Matches(@"^[a-zA-Z0-9]+$").WithMessage("Không hợp lệ.")
                .MinimumLength(2).WithMessage("Tối thiểu 2 kí tự.")
                                  .MustAsync(async (id, cancellation) =>
                                  {
                                      bool result = true;
                                      if (!String.IsNullOrEmpty(id))
                                      if (!String.IsNullOrEmpty(id))
                                      {
                                          result = await _inventoryService.ContainsICode(id);
                                      }
                                      return result;
                                  }).When(x => x.IsTypeUpdate == 0).WithMessage("Đã tồn tại.");

                RuleFor(x => x.IName).NotEmpty().WithMessage("Không được trống.");
                RuleFor(x => x.ITypeCode).NotEmpty().WithMessage("Không được trống.");
                RuleFor(x => x.IClsCode).NotEmpty().WithMessage("Không được trống.");
                RuleFor(x => x.IGrpCode).NotEmpty().WithMessage("Không được trống.");
                RuleFor(x => x.IUnitCode).NotEmpty().WithMessage("Không được trống.");

            });
        }
    }

    public class ItemsGroupValidator : AbstractValidator<ItemsGroupVM>
    {
        public ItemsGroupValidator(InventoryService _inventoryService)
        {
            When(x => x.IsTypeUpdate != 2, () =>
            {
                RuleFor(x => x.IGrpCode).NotEmpty().WithMessage("Không được trống.")
                .Matches(@"^[a-zA-Z0-9]+$").WithMessage("Không hợp lệ.")
                .MinimumLength(2).WithMessage("Tối thiểu 2 kí tự.")
                .MustAsync(async (id, cancellation) =>
                {
                    bool result = true;
                    if (!String.IsNullOrEmpty(id))
                    {
                        result = await _inventoryService.ContainsIGrpCode(id);
                    }
                    return result;
                }).When(x => x.IsTypeUpdate == 0).WithMessage("Đã tồn tại.");

                RuleFor(x => x.IGrpName).NotEmpty().WithMessage("Không được trống.");
                RuleFor(x => x.IClsCode).NotEmpty().WithMessage("Không được trống.");

            });
        }
    }

    public class ItemsUnitValidator : AbstractValidator<ItemsUnitVM>
    {
        public ItemsUnitValidator(InventoryService _inventoryService)
        {
            When(x => x.IsTypeUpdate != 2, () =>
            {
                RuleFor(x => x.IUnitCode).NotEmpty().WithMessage("Không được trống.")
                .Matches(@"^[a-zA-Z0-9]+$").WithMessage("Không hợp lệ.")
                .MinimumLength(2).WithMessage("Tối thiểu 2 kí tự.")
                .Must((x, id) => _inventoryService.ContainsIUnitCode(x.IUnitCode).Result).When(x => x.IsTypeUpdate == 0).WithMessage("Đã tồn tại.");

                RuleFor(x => x.IUnitName).NotEmpty().WithMessage("Không được trống.");
            });
        }
    }

    public class StockVoucherValidator : AbstractValidator<StockVoucherVM>
    {
        public StockVoucherValidator()
        {
            When(x => x.VTypeID == "STOCK_Input", () =>
            {
                When(x => x.IsTypeUpdate != 2, () =>
                {
                    RuleFor(x => x.VDate).NotEmpty().WithMessage("Không được trống.");

                    RuleFor(x => x.VDesc).NotEmpty().WithMessage("Không được trống.");
                });

                When(x => x.VSubTypeID == "STOCK_Input_Purchasing", () =>
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

            When(x => x.VTypeID == "STOCK_Output", () =>
            {
                When(x => x.IsTypeUpdate != 2, () =>
                {
                    RuleFor(x => x.VDate).NotEmpty().WithMessage("Không được trống.");

                    RuleFor(x => x.VDesc).NotEmpty().WithMessage("Không được trống.");
                });
            });

            When(x => x.VTypeID == "STOCK_Transfer", () =>
            {
                When(x => x.IsTypeUpdate != 2, () =>
                {
                    RuleFor(x => x.VDate).NotEmpty().WithMessage("Không được trống.");

                    RuleFor(x => x.VDesc).NotEmpty().WithMessage("Không được trống.");
                });

                When(x => x.VSubTypeID == "STOCK_Transfer_Purchasing", () =>
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

            When(x => x.VTypeID == "STOCK_InventoryCheck", () =>
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