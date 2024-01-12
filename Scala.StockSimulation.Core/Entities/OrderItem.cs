namespace Scala.StockSimulation.Core.Entities
{
    public class OrderItem : BaseEntity
    {
        public int Quantity { get; set; }
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
        public Guid OrderId { get; set; }
        public Order Order { get; set; }
    }
}