namespace APBD7.Models;

public class ProductWarehouse
{
    private int IdProductWarehouse { set; get; }
    private int IdWarehouse { set; get; }
    private int IdProduct { set; get; }
    private int IdOrder { set; get; }
    private int Amount { set; get; }
    private double Price { set; get; }
    private DateTime CreatedAt { set; get; }

}