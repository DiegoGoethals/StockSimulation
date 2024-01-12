namespace Scala.StockSimulation.Core.Entities
{
    public class Supplier : BaseEntity
    {
        public string Name { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}