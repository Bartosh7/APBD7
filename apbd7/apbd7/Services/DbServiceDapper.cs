using System.Data;
using System.Data.SqlClient;
using APBD7.DTOs;
using APBD7.Models;
using Dapper;

namespace APBD7.Services;

public interface IDbServiceDapper
{
    Task<int> AddProductToWarehouse(CreateProductWarehouseRequest createProductWarehouseRequest);
    Task<Product?> GetProduct(int id);
    Task<ProductWarehouse?> IsOrderExistsInWarehouseById(int orderId);
    Task<int?> GetLastProductWarehouseId();
    Task<int?> GetOrderId(int productId, int amount, DateTime dateTime);
    Task<Warehouse?> GetWarehouseById(int id);
    Task<Order?> GetOrderObjectById(int productId, int amount, DateTime dateTime);
}

public class DbServiceDapper(IConfiguration configuration) : IDbServiceDapper
{
    private async Task<SqlConnection> GetConnection()
    {
        var connection = new SqlConnection(configuration.GetConnectionString("Default"));
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        return connection;
    }

    public async Task<Product?> GetProduct(int id)
    {
        await using var connection = await GetConnection();

        var result =
            await connection.QueryFirstOrDefaultAsync<Product>(
                "SELECT * FROM Product WHERE IdProduct = @Id",
                new { Id = id });

        return result;
    }


    public async Task<int?> GetOrderId(int productId, int amount, DateTime dateTime)
    {
        await using var connection = await GetConnection();

        var result =
            await connection.QueryFirstOrDefaultAsync<int>(
                "SELECT IdOrder FROM [Order] WHERE IdProduct = @IdProduct AND Amount = @Amount AND  CreatedAt<@Date",
                new { IdProduct = productId, Amount = amount, Date = dateTime });

        return result;
    }
    
    public async Task<Order?> GetOrderObjectById(int productId, int amount, DateTime dateTime)
    {
        await using var connection = await GetConnection();

        var result =
            await connection.QueryFirstOrDefaultAsync<Order>(
                "SELECT * FROM [Order] WHERE IdProduct = @IdProduct AND Amount = @Amount AND  CreatedAt<@Date",
                new { IdProduct = productId, Amount = amount, Date = dateTime });

        return result;
    }

    public async Task<Warehouse?> GetWarehouseById(int id)
    {
        await using var connection = await GetConnection();

        var result =
            await connection.QueryFirstOrDefaultAsync<Warehouse>(
                "SELECT * FROM Warehouse WHERE IdWarehouse = @IdWarehouse",
                new { IdWarehouse = id});

        return result;
    }

    public async Task<ProductWarehouse?> IsOrderExistsInWarehouseById(int orderId)
    {
        await using var connection = await GetConnection();

        var result =
            await connection.QueryFirstOrDefaultAsync<ProductWarehouse>(
                "SELECT * FROM Product_Warehouse WHERE IdOrder = @IdOrder",
                new { IdOrder = orderId });

        return result;
    }

    public async Task<int?> GetLastProductWarehouseId()
    {
        await using var connection = await GetConnection();

        var maxId = await connection.QueryFirstOrDefaultAsync<int?>(
            "SELECT MAX(IdProductWarehouse) FROM Product_Warehouse");

        return maxId;
    }


    public async Task<int> AddProductToWarehouse(CreateProductWarehouseRequest createProductWarehouseRequest)
    {
        await using var connection = await GetConnection();
        await using var transaction = await connection.BeginTransactionAsync();
        

        var product = await GetProduct(createProductWarehouseRequest.IdProduct);
        var order = await GetOrderId(createProductWarehouseRequest.IdProduct, createProductWarehouseRequest.Amount,
            createProductWarehouseRequest.CreatedAt);
        
        

        

        try
        {
            await connection.ExecuteAsync(
                "INSERT INTO Product_Warehouse (IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt) VALUES (@IdWar, @IdProd, @IdOrd, @Am, @Pr, @CrAt)",
                new
                {
                    IdWar = createProductWarehouseRequest.IdWarehouse,
                    IdProd = createProductWarehouseRequest.IdProduct,
                    IdOrd = order,
                    Am = createProductWarehouseRequest.Amount,
                    Pr = product.Price * createProductWarehouseRequest.Amount,
                    CrAt = createProductWarehouseRequest.CreatedAt
                },
                transaction);

            await connection.ExecuteAsync(
                "UPDATE [Order] SET FulfilledAt = @FulAt WHERE IdOrder = @Id",
                new { FulAt = createProductWarehouseRequest.CreatedAt, Id = order },
                transaction);


            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }

        var lastProductWarehouseId = await GetLastProductWarehouseId();
        return (int)lastProductWarehouseId;
    }
}