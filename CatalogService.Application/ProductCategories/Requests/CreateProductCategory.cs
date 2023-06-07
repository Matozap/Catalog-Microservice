using System.Runtime.Serialization;
using CatalogService.Application.ProductCategories.Responses;
using FluentValidation;
using MediatrBuilder.Interfaces;

namespace CatalogService.Application.ProductCategories.Requests;

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
