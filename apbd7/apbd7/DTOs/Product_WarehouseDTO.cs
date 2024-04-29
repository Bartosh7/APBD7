namespace APBD7.DTOs;

public record GetProductWarehouse(int IdProductWarehouse, int IdWarehouse, int IdProduct, int IdOrder, int Amount,
    double Price, DateTime CreatedAt);

public record CreateProductWarehouseRequest(int IdProduct, int IdWarehouse, int Amount, DateTime CreatedAt); 