using FluentValidation;
using CatalogService.Application.Interfaces;

namespace CatalogService.Application.Handlers.ProductImages.v1.Requests;

public class GetAllProductImages : IQuery<object>
{
    public string ProductId { get; init; }
}

public class GetAllProductImagesValidator : AbstractValidator<GetAllProductImages>
{
    public GetAllProductImagesValidator()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.ProductId)
            .NotNull().NotEmpty().WithMessage("Product id is required")
            .MaximumLength(36).WithMessage("Product id  cannot exceed 36 characters");
    }
}
