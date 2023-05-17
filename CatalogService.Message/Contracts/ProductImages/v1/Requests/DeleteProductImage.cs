using System.Runtime.Serialization;
using CatalogService.Message.Contracts.Common.Interfaces;
using FluentValidation;

namespace CatalogService.Message.Contracts.ProductImages.v1.Requests;

[DataContract]
public class DeleteProductImage : ICommand<string>
{
    [DataMember(Order = 1)]
    public string Id { get; init; }
}

public class DeleteProductImageValidator : AbstractValidator<DeleteProductImage>
{
    public DeleteProductImageValidator()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.Id)
            .NotNull().NotEmpty().WithMessage("Id is required")
            .MaximumLength(36).WithMessage("Id cannot exceed 36 characters");
    }
}
