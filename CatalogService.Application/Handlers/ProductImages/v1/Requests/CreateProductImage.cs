using FluentValidation;
using CatalogService.Application.Interfaces;
using CatalogService.Message.Contracts.ProductImages.v1;

namespace CatalogService.Application.Handlers.ProductImages.v1.Requests;

public class CreateProductImage : ICommand<ProductImageData>
{
    public ProductImageData Details { get; init; }
}

public class CreateProductImageValidator : AbstractValidator<CreateProductImage>
{
    public CreateProductImageValidator()
    {
        RuleFor(x => x.Details).NotNull();
        RuleFor(x => x.Details.Name)
            .NotNull().NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name cannot exceed 200 characters");
        RuleFor(x => x.Details.Code)
            .NotNull().NotEmpty().WithMessage("Code is required")
            .MaximumLength(36).WithMessage("Code cannot exceed 36 characters");
        RuleFor(x => x.Details.ProductId)
            .NotNull().NotEmpty().WithMessage("Product id is required")
            .MaximumLength(36).WithMessage("Product id cannot exceed 36 characters");
    }
}
