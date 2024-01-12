namespace Scala.StockSimulation.Core.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ArticleNumber { get; set; }
        public int InitialStock { get; set; }
        public int InitialMinimumStock { get; set; }
        public int InitialMaximumStock { get; set; }
        public Guid SupplierId { get; set; }
        public Supplier Supplier { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
        public ICollection<UserProductState> UserProductStates { get; set; }
    }
}