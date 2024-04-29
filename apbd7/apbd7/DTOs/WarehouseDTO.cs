namespace APBD7.DTOs;

public class WarehouseDTO
{
    public record getWarehouse(int IdWarehouse, string name, string address);
}