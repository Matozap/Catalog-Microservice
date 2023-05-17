using System.Runtime.Serialization;
using CatalogService.Message.Contracts.Common.Interfaces;
using FluentValidation;

namespace CatalogService.Message.Contracts.ProductCategories.v1.Requests;

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
