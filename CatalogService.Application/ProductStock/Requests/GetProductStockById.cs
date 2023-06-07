using System.Runtime.Serialization;
using CatalogService.Application.ProductStock.Responses;
using FluentValidation;
using MediatrBuilder.Interfaces;

namespace CatalogService.Application.ProductStock.Requests;

[DataContract]
public class GetProductStockById : IQuery<ProductStockData>
{
    [DataMember(Order = 1)]
    public string Id { get; init; }
}

public class GetProductStockByIdValidator : AbstractValidator<GetProductStockById>
{
    public GetProductStockByIdValidator()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.Id)
            .NotNull().NotEmpty().WithMessage("Id is required")
            .MaximumLength(36).WithMessage("Id cannot exceed 36 characters");
    }
}
