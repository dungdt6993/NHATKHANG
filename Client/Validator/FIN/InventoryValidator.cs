using D69soft.Client.Services.FIN;
using D69soft.Server.Services.HR;
using D69soft.Shared.Models.ViewModels.FIN;
using FluentValidation;

namespace D69soft.Client.Validator.FIN
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
                RuleFor(x => x.IUnitCode).NotEmpty().WithMessage("Không được trống.");

            });
        }
    }

    public class ItemsClassValidator : AbstractValidator<ItemsClassVM>
    {
        public ItemsClassValidator(InventoryService _inventoryService)
        {
            When(x => x.IsTypeUpdate != 2, () =>
            {
                RuleFor(x => x.IClsCode).NotEmpty().WithMessage("Không được trống.")
                .Matches(@"^[a-zA-Z0-9]+$").WithMessage("Không hợp lệ.")
                .MinimumLength(2).WithMessage("Tối thiểu 2 kí tự.")
                .MustAsync(async (id, cancellation) =>
                {
                    bool result = true;
                    if (!String.IsNullOrEmpty(id))
                    {
                        result = await _inventoryService.ContainsIClsCode(id);
                    }
                    return result;
                }).When(x => x.IsTypeUpdate == 0).WithMessage("Đã tồn tại.");

                RuleFor(x => x.IClsName).NotEmpty().WithMessage("Không được trống.");

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
                  .MustAsync(async (id, cancellation) =>
                  {
                      bool result = true;
                      if (!String.IsNullOrEmpty(id))
                          if (!String.IsNullOrEmpty(id))
                          {
                              result = await _inventoryService.ContainsIUnitCode(id);
                          }
                      return result;
                  }).When(x => x.IsTypeUpdate == 0).WithMessage("Đã tồn tại.");

                RuleFor(x => x.IUnitName).NotEmpty().WithMessage("Không được trống.");
            });
        }
    }

    public class StockValidator : AbstractValidator<StockVM>
    {
        public StockValidator(InventoryService _inventoryService)
        {
            When(x => x.IsTypeUpdate != 2, () =>
            {
                RuleFor(x => x.StockCode).NotEmpty().WithMessage("Không được trống.")
                .Matches(@"^[a-zA-Z0-9]+$").WithMessage("Không hợp lệ.")
                .MinimumLength(2).WithMessage("Tối thiểu 2 kí tự.")
                                  .MustAsync(async (id, cancellation) =>
                                  {
                                      bool result = true;
                                      if (!String.IsNullOrEmpty(id))
                                          if (!String.IsNullOrEmpty(id))
                                          {
                                              result = await _inventoryService.ContainsStockCode(id);
                                          }
                                      return result;
                                  }).When(x => x.IsTypeUpdate == 0).WithMessage("Đã tồn tại.");

                RuleFor(x => x.StockName).NotEmpty().WithMessage("Không được trống.");

            });
        }
    }

}