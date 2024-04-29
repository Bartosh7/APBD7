namespace APBD7.Models;

public class Order
{
    private int IdOrder { set; get; }
    private int IdProduct { set; get; }
    private double Amount { set; get; }
    private DateTime CreatedAt { set; get; }
    private DateTime FullfilledAt { set; get; }
    
}