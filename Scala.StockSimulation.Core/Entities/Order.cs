namespace Scala.StockSimulation.Core.Entities
{
    public class Order : BaseEntity
    {
        public DateTime DateDelivered { get; set; }
        public bool IsTemporary { get; set; }
        public Guid ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public Guid OrderTypeId { get; set; }
        public OrderType OrderType { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}