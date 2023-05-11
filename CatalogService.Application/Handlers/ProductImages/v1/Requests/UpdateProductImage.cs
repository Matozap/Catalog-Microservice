using FluentValidation;
using CatalogService.Application.Interfaces;
using CatalogService.Message.Contracts.ProductImages.v1;

namespace CatalogService.Application.Handlers.ProductImages.v1.Requests;

public class UpdateProductImage : ICommand<ProductImageData>
{
    public ProductImageData Details { get; init; }
}

public class UpdateProductImageValidator : AbstractValidator<UpdateProductImage>
{
    public UpdateProductImageValidator()
    {
        RuleFor(x => x.Details).NotNull();
        RuleFor(x => x.Details.Id)
            .NotNull().NotEmpty().WithMessage("Id is required")
            .MaximumLength(36).WithMessage("Id cannot exceed 36 characters");
    }
}
