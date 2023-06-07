using System.Runtime.Serialization;
using CatalogService.Application.ProductCategories.Responses;
using FluentValidation;
using MediatrBuilder.Interfaces;

namespace CatalogService.Application.ProductCategories.Requests;

[DataContract]
public class SoftDeleteProductCategory : ICommand<ProductCategoryData>
{
    [DataMember(Order = 1)]
    public string Id { get; init; }
}

public class SoftDeleteProductCategoryValidator : AbstractValidator<SoftDeleteProductCategory>
{
    public SoftDeleteProductCategoryValidator()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.Id)
            .NotNull().NotEmpty().WithMessage("Id is required")
            .MaximumLength(36).WithMessage("Id cannot exceed 36 characters");
    }
}
