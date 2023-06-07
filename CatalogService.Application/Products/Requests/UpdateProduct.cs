using System.Runtime.Serialization;
using CatalogService.Application.Products.Responses;
using FluentValidation;
using MediatrBuilder.Interfaces;

namespace CatalogService.Application.Products.Requests;

[DataContract]
public class UpdateProduct : ICommand<ProductData>
{
    [DataMember(Order = 1)]
    public ProductData Details { get; init; }
}

public class UpdateProductValidator : AbstractValidator<UpdateProduct>
{
    public UpdateProductValidator()
    {
        RuleFor(x => x.Details).NotNull();
        RuleFor(x => x.Details.Id)
            .NotNull().NotEmpty().WithMessage("Id is required")
            .MaximumLength(36).WithMessage("Id cannot exceed 36 characters");
    }
}
