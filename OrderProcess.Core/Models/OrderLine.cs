namespace OrderProcess.Core.Models;

public class OrderLine  
{
    public Guid Id { get; set; }
    public string Sku { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}