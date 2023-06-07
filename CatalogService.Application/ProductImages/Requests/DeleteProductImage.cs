using System.Runtime.Serialization;
using CatalogService.Application.ProductImages.Responses;
using FluentValidation;
using MediatrBuilder.Interfaces;

namespace CatalogService.Application.ProductImages.Requests;

[DataContract]
public class DeleteProductImage : ICommand<ProductImageData>
{
    [DataMember(Order = 1)]
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
