using FluentValidation;
using CatalogService.Application.Interfaces;

namespace CatalogService.Application.Handlers.ProductImages.v1.Requests;

public class DeleteProductImage : ICommand<string>
{
    public string Id { get; init; }
}

public class DeleteProductImageValidator : AbstractValidator<DeleteProductImage>
{
    public DeleteProductImageValidator()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.Id)
            .NotNull().NotEmpty().WithMessage("Id is required")
            .MaximumLength(36).WithMessage("Id cannot exceed 36 characters");
    }
}
