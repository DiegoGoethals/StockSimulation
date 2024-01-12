namespace Scala.StockSimulation.Web.ViewModels
{
    public class ProductViewModel
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public bool IsSelected { get; set; }

        public string ArticleNumber { get; set; }
    }
}