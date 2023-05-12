using CatalogService.Application.Interfaces;
using CatalogService.Message.Contracts.ProductCategories.v1;
using FluentValidation;

namespace CatalogService.Application.Handlers.ProductCategories.v1.Requests;

public class GetProductCategoryById : IQuery<ProductCategoryData>
{
    public string Id { get; init; }
}

public class GetProductCategoryByIdValidator : AbstractValidator<GetProductCategoryById>
{
    public GetProductCategoryByIdValidator()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.Id)
            .NotNull().NotEmpty().WithMessage("Id is required")
            .MaximumLength(36).WithMessage("Id cannot exceed 36 characters");
    }
}
