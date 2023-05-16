using FluentValidation;
using CatalogService.Application.Interfaces;

namespace CatalogService.Application.Handlers.ProductStock.v1.Requests;

public class DeleteProductStock : ICommand<string>
{
    public string Id { get; init; }
}

public class DeleteProductStockValidator : AbstractValidator<DeleteProductStock>
{
    public DeleteProductStockValidator()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.Id)
            .NotNull().NotEmpty().WithMessage("Id is required")
            .MaximumLength(36).WithMessage("Id cannot exceed 36 characters");
    }
}
