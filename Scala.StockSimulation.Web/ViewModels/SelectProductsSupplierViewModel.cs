using Microsoft.AspNetCore.Mvc.Rendering;

namespace Scala.StockSimulation.Web.ViewModels
{
    public class SelectProductsSupplierViewModel
    {
        public Guid SupplierId { get; set; }
        public List<SelectListItem> Supplier { get; set; }

        public List<ProductViewModel> Products { get; set; }

        public string ArticleNumber { get; set; }
    }
}