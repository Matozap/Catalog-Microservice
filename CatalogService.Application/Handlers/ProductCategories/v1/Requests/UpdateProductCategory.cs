using CatalogService.Application.Interfaces;
using CatalogService.Message.Contracts.ProductCategories.v1;
using FluentValidation;

namespace CatalogService.Application.Handlers.ProductCategories.v1.Requests;

public class UpdateProductCategory : ICommand<ProductCategoryData>
{
    public ProductCategoryData Details { get; init; }
}

public class UpdateProductCategoryValidator : AbstractValidator<UpdateProductCategory>
{
    public UpdateProductCategoryValidator()
    {
        RuleFor(x => x.Details).NotNull();
        RuleFor(x => x.Details.Id)
            .NotNull().NotEmpty().WithMessage("Id is required")
            .MaximumLength(36).WithMessage("Id cannot exceed 36 characters");
    }
}
