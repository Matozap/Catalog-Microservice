using System.Runtime.Serialization;
using CatalogService.Application.ProductCategories.Responses;
using FluentValidation;
using MediatrBuilder.Interfaces;

namespace CatalogService.Application.ProductCategories.Requests;

[DataContract]
public class UpdateProductCategory : ICommand<ProductCategoryData>
{
    [DataMember(Order = 1)]
    public ProductCategoryData Details { get; init; }
}

public class UpdateProductCategoryValidator : AbstractValidator<UpdateProductCategory>
{
    public UpdateProductCategoryValidator()
    {
        RuleFor(x => x.Details).NotNull();
        RuleFor(x => x.Details.Id)
            .NotNull().NotEmpty().WithMessage("Id is required")
            .MaximumLength(36).WithMessage("Id cannot exceed 36 characters");
    }
}
