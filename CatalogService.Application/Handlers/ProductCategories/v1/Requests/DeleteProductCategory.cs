using CatalogService.Application.Interfaces;
using FluentValidation;

namespace CatalogService.Application.Handlers.ProductCategories.v1.Requests;

public class DeleteProductCategory : ICommand<string>
{
    public string Id { get; init; }
}

public class DeleteProductCategoryValidator : AbstractValidator<DeleteProductCategory>
{
    public DeleteProductCategoryValidator()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.Id)
            .NotNull().NotEmpty().WithMessage("Id is required")
            .MaximumLength(36).WithMessage("Id cannot exceed 36 characters");
    }
}
