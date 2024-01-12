namespace Scala.StockSimulation.Web.ViewModels
{
    public class UserStateOverviewViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int PhysicalStock { get; set; }
        public int FictionalStock { get; set; }
        public int MinimumStock { get; set; }
        public int MaximumStock { get; set; }
        public int SoonAvailableStock { get; set; }
        public int ReservedStock { get; set; }
        public int Quantity { get; set; }
        public string Product { get; set; }
    }
}