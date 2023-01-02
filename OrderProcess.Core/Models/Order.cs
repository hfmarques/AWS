namespace OrderProcess.Core.Models;

public class Order
{
    public Guid Id { get; set; }
    public Customer Customer { get; set; }
    public List<OrderLine> Lines { get; set; }
}