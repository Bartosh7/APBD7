using APBD7.DTOs;
using FluentValidation;

namespace APBD7.Validators;

public class CreateProductWarehouseValidator : AbstractValidator<CreateProductWarehouseRequest>
{
    public CreateProductWarehouseValidator()
    {
        RuleFor(e => e.IdProduct).NotEmpty();
        RuleFor(e => e.IdWarehouse).NotEmpty();
        RuleFor(e => e.Amount).GreaterThan(0);
        RuleFor(e => e.CreatedAt).NotEmpty();
    }
}