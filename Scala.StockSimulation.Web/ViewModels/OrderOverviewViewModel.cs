namespace Scala.StockSimulation.Web.ViewModels
{
    public class OrderOverviewViewModel
    {
        public IEnumerable<UserStateOverviewViewModel> UserProductState { get; set; }
        public string OrderType { get; set; }
    }
}