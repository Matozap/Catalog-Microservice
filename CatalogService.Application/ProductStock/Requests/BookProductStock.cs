using System.Runtime.Serialization;
using CatalogService.Application.ProductStock.Responses;
using FluentValidation;
using MediatrBuilder.Interfaces;

namespace CatalogService.Application.ProductStock.Requests;

[DataContract]
public class BookProductStock : ICommand<BookProductStockResponse>
{
    [DataMember(Order = 1)]
    public string ProductId { get; init; }
    [DataMember(Order = 2)]
    public decimal Value { get; init; }
}

public class BookProductStockValidator : AbstractValidator<BookProductStock>
{
    public BookProductStockValidator()
    {
        RuleFor(x => x.ProductId).NotNull();
        RuleFor(x => x.Value)
            .GreaterThan(0).WithMessage("Booked value must be a positive number and greater than 0");
    }
}
