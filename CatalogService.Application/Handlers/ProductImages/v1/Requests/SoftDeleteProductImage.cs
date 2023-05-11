using FluentValidation;
using CatalogService.Application.Interfaces;

namespace CatalogService.Application.Handlers.ProductImages.v1.Requests;

public class SoftDeleteProductImage : ICommand<string>
{
    public string Id { get; init; }
}

public class SoftDeleteProductImageValidator : AbstractValidator<SoftDeleteProductImage>
{
    public SoftDeleteProductImageValidator()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.Id)
            .NotNull().NotEmpty().WithMessage("Id is required")
            .MaximumLength(36).WithMessage("Id cannot exceed 36 characters");
    }
}
