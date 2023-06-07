using System.Runtime.Serialization;
using FluentValidation;
using MediatrBuilder.Interfaces;

namespace CatalogService.Application.ProductStock.Requests;

[DataContract]
public class SoftDeleteProductStock : ICommand<string>
{
    [DataMember(Order = 1)]
    public string Id { get; init; }
}

public class SoftDeleteProductStockValidator : AbstractValidator<SoftDeleteProductStock>
{
    public SoftDeleteProductStockValidator()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.Id)
            .NotNull().NotEmpty().WithMessage("Id is required")
            .MaximumLength(36).WithMessage("Id cannot exceed 36 characters");
    }
}
