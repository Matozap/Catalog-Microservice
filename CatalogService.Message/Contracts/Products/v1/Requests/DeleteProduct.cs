using System.Runtime.Serialization;
using CatalogService.Message.Contracts.Common.Interfaces;
using FluentValidation;

namespace CatalogService.Message.Contracts.Products.v1.Requests;

[DataContract]
public class DeleteProduct : ICommand<string>
{
    [DataMember(Order = 1)]
    public string Id { get; init; }
}

public class DeleteProductValidator : AbstractValidator<DeleteProduct>
{
    public DeleteProductValidator()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.Id)
            .NotNull().NotEmpty().WithMessage("Id is required")
            .MaximumLength(36).WithMessage("Id cannot exceed 36 characters");
    }
}
