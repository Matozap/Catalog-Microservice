using System.Runtime.Serialization;
using CatalogService.Application.Products.Responses;
using FluentValidation;
using MediatrBuilder.Interfaces;

namespace CatalogService.Application.Products.Requests;

[DataContract]
public class SoftDeleteProduct : ICommand<ProductData>
{
    [DataMember(Order = 1)]
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
