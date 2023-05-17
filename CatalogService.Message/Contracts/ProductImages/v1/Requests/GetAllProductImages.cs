using System.Runtime.Serialization;
using CatalogService.Message.Contracts.Common.Interfaces;
using FluentValidation;

namespace CatalogService.Message.Contracts.ProductImages.v1.Requests;

[DataContract]
public class GetAllProductImages : IQuery<object>
{
    [DataMember(Order = 1)]
    public string ProductId { get; init; }
}

public class GetAllProductImagesValidator : AbstractValidator<GetAllProductImages>
{
    public GetAllProductImagesValidator()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.ProductId)
            .NotNull().NotEmpty().WithMessage("Product id is required")
            .MaximumLength(36).WithMessage("Product id  cannot exceed 36 characters");
    }
}
