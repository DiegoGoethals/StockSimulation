using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Scala.StockSimulation.Web.Data;
using Scala.StockSimulation.Web.ViewModels;

namespace Scala.StockSimulation.Web.Services
{
    public class FormHelperService
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public FormHelperService(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<List<SelectListItem>> GetSuppliers()
        {
            return await _applicationDbContext.Suppliers.Where(s => s.Products.Any())
                .Select(s => new SelectListItem
                {
                    Text = s.Name,
                    Value = s.Id.ToString()
                }).ToListAsync();
        }

        public async Task<List<ProductViewModel>> GetProductsBySupplier(Guid supplierId)
        {
            return await _applicationDbContext.Products
                .Where(p => p.SupplierId == supplierId)
                .Select(p => new ProductViewModel
                {
                    ProductId = p.Id,
                    ProductName = p.Name,
                    ArticleNumber = p.ArticleNumber,
                }).ToListAsync();
        }
    }
}