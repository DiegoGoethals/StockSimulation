namespace Scala.StockSimulation.Web.ViewModels
{
    public class UserProductStateViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int PhysicalStock { get; set; }
        public int FictionalStock { get; set; }
        public int MinimumStock { get; set; }
        public int MaximumStock { get; set; }
        public int SoonAvailableStock { get; set; }
        public int ReservedStock { get; set; }
        public string TransactionType { get; set; }
        public OrderItemViewModel OrderItem { get; set; }
        public int QuantityOrdered { get; set; }
        public string Status { get; set; }
        public DateTime Date { get; set; }
        public string UserName { get; set; }

        public Guid OrderId { get; set; }
    }
}