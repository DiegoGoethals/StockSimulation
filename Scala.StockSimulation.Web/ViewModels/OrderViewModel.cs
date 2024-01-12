using Scala.StockSimulation.Core.Entities;

namespace Scala.StockSimulation.Web.ViewModels
{
    public class OrderViewModel
    {
        public Guid OrderId { get; set; }
        public DateTime DateDelivered { get; set; }
        public Guid ApplicationUserId { get; set; }
        public Guid OrderTypeId { get; set; }
        public List<OrderItem> OrderItems { get; set; }

        public DateTime Created { get; set; }

        public string UserName { get; set; }
        public string OrderType { get; set; }
        public string Status { get; set; }
        public bool IsDelivered { get; set; }
    }
}