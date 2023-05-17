using System.Runtime.Serialization;
using CatalogService.Message.Contracts.Common.Interfaces;
using FluentValidation;

namespace CatalogService.Message.Contracts.ProductCategories.v1.Requests;

[DataContract]
public class CreateProductCategory : ICommand<ProductCategoryData>
{
    [DataMember(Order = 1)]
    public ProductCategoryData Details { get; init; }
}

public class CreateProductCategoryValidator : AbstractValidator<CreateProductCategory>
{
    public CreateProductCategoryValidator()
    {
        RuleFor(x => x.Details).NotNull();
        RuleFor(x => x.Details.Name)
            .NotNull().NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name cannot exceed 200 characters");
    }
}
