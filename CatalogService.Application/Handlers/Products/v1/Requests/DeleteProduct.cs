using FluentValidation;
using CatalogService.Application.Interfaces;

namespace CatalogService.Application.Handlers.Products.v1.Requests;

public class DeleteProduct : ICommand<string>
{
    public string Id { get; init; }
}

public class DeleteProductValidator : AbstractValidator<DeleteProduct>
{
    public DeleteProductValidator()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.Id)
            .NotNull().NotEmpty().WithMessage("Id is required")
            .MaximumLength(36).WithMessage("Id cannot exceed 36 characters");
    }
}
