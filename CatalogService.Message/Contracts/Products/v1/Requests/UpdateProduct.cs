using System.Runtime.Serialization;
using CatalogService.Message.Contracts.Common.Interfaces;
using FluentValidation;

namespace CatalogService.Message.Contracts.Products.v1.Requests;

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
