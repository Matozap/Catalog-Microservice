using FluentValidation;
using CatalogService.Application.Interfaces;

namespace CatalogService.Application.Handlers.ProductStock.v1.Requests;

public class SoftDeleteProductStock : ICommand<string>
{
    public string Id { get; init; }
}

public class SoftDeleteProductStockValidator : AbstractValidator<SoftDeleteProductStock>
{
    public SoftDeleteProductStockValidator()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.Id)
            .NotNull().NotEmpty().WithMessage("Id is required")
            .MaximumLength(36).WithMessage("Id cannot exceed 36 characters");
    }
}
