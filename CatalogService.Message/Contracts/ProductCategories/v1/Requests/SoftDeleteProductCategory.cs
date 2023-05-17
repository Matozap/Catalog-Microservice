using System.Runtime.Serialization;
using CatalogService.Message.Contracts.Common.Interfaces;
using FluentValidation;

namespace CatalogService.Message.Contracts.ProductCategories.v1.Requests;

[DataContract]
public class SoftDeleteProductCategory : ICommand<string>
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
