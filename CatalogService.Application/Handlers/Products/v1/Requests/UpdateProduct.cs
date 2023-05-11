using FluentValidation;
using CatalogService.Application.Interfaces;
using CatalogService.Message.Contracts.Products.v1;

namespace CatalogService.Application.Handlers.Products.v1.Requests;

public class UpdateProduct : ICommand<ProductData>
{
    public ProductData Details { get; init; }
}

public class UpdateProductValidator : AbstractValidator<UpdateProduct>
{
    public UpdateProductValidator()
    {
        RuleFor(x => x.Details).NotNull();
        RuleFor(x => x.Details.Id)
            .NotNull().NotEmpty().WithMessage("Id is required")
            .MaximumLength(36).WithMessage("Id cannot exceed 36 characters");
    }
}
