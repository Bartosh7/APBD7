using System.Data.SqlClient;
using APBD7.DTOs;
using APBD7.Models;
using APBD7.Services;
using Dapper;
using FluentValidation;

namespace APBD7.Endpoints;

public static class ProductWarehouseDapperEndpoints
{
    public static void RegisterWarehouseDapperEndpoints(this WebApplication app)
    {
        app.MapPost("productWarehouse", AddProductWarehouse);
    }

    private static async Task<IResult> AddProductWarehouse(
        CreateProductWarehouseRequest request,
        IDbServiceDapper db,
        IValidator<CreateProductWarehouseRequest> validator
    )
    {
        // Validate request
        var validate = await validator.ValidateAsync(request);
        if (!validate.IsValid)
        {
            return Results.ValidationProblem(validate.ToDictionary());
        }
        
        if (request.Amount <= 0)
        {
            return Results.BadRequest();
        }
        var product = await db.GetProduct(request.IdProduct);
        if (product == null)
        {
            return Results.NotFound($"Product with id {request.IdProduct} not found");
        }

        var order = await db.GetOrderObjectById(request.IdProduct, request.Amount,
            request.CreatedAt);
        if (order == null)
        {
            return Results.NotFound($"Can't find order");
        }
        
        
        
        var orderId = await db.GetOrderId(request.IdProduct, request.Amount,
            request.CreatedAt);

        
        
        if (await db.IsOrderExistsInWarehouseById((int)orderId) != null)
        {
            return Results.BadRequest($"Order already exists");
        }
        var warehouse = await db.GetWarehouseById(request.IdWarehouse);
        if (warehouse == null)
        {
            return Results.NotFound($"Warehouse with id {request.IdWarehouse} not found");
        }
        

        var result = await db.AddProductToWarehouse(request);

        return Results.Created($"/product-warehouse/{result}", result);
    }
}





