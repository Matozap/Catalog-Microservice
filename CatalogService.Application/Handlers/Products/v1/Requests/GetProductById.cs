using FluentValidation;
using CatalogService.Application.Interfaces;
using CatalogService.Message.Contracts.Products.v1;

namespace CatalogService.Application.Handlers.Products.v1.Requests;

public class GetProductById : IQuery<ProductData>
{
    public string Id { get; init; }
}

public class GetProductByIdValidator : AbstractValidator<GetProductById>
{
    public GetProductByIdValidator()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.Id)
            .NotNull().NotEmpty().WithMessage("Id is required")
            .MaximumLength(36).WithMessage("Id cannot exceed 36 characters");
    }
}
