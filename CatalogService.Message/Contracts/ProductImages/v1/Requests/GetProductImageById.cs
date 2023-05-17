using System.Runtime.Serialization;
using CatalogService.Message.Contracts.Common.Interfaces;
using FluentValidation;

namespace CatalogService.Message.Contracts.ProductImages.v1.Requests;

[DataContract]
public class GetProductImageById : IQuery<object>
{
    [DataMember(Order = 1)]
    public string Id { get; init; }
}

public class GetProductImageByIdValidator : AbstractValidator<GetProductImageById>
{
    public GetProductImageByIdValidator()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.Id)
            .NotNull().NotEmpty().WithMessage("Id is required")
            .MaximumLength(36).WithMessage("Id cannot exceed 36 characters");
    }
}
