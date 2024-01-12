namespace Scala.StockSimulation.Web.ViewModels
{
    public class DisplayProductsViewModel
    {
        public Guid SupplierId { get; set; }
        public List<ProductViewModel> Products { get; set; }
    }
}