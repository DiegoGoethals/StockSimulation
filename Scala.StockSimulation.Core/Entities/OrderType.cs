namespace Scala.StockSimulation.Core.Entities
{
    public class OrderType : BaseEntity
    {
        public string Name { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}