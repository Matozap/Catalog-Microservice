using FluentValidation;
using CatalogService.Application.Interfaces;

namespace CatalogService.Application.Handlers.Products.v1.Requests;

public class SoftDeleteProduct : ICommand<string>
{
    public string Id { get; init; }
}

public class SoftDeleteProductValidator : AbstractValidator<SoftDeleteProduct>
{
    public SoftDeleteProductValidator()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.Id)
            .NotNull().NotEmpty().WithMessage("Id is required")
            .MaximumLength(36).WithMessage("Id cannot exceed 36 characters");
    }
}
