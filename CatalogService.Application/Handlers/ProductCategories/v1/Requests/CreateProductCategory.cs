using CatalogService.Application.Interfaces;
using CatalogService.Message.Contracts.ProductCategories.v1;
using FluentValidation;

namespace CatalogService.Application.Handlers.ProductCategories.v1.Requests;

public class CreateProductCategory : ICommand<ProductCategoryData>
{
    public ProductCategoryData Details { get; init; }
}

public class CreateProductCategoryValidator : AbstractValidator<CreateProductCategory>
{
    public CreateProductCategoryValidator()
    {
        RuleFor(x => x.Details).NotNull();
        RuleFor(x => x.Details.Name)
            .NotNull().NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name cannot exceed 200 characters");
    }
}
