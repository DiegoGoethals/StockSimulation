using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scala.StockSimulation.Core.Entities;
using Scala.StockSimulation.Web.Data;
using Scala.StockSimulation.Web.Services;
using Scala.StockSimulation.Web.ViewModels;

namespace Scala.StockSimulation.Web.Controllers
{
    [Authorize(Roles = "Admin, Student")]
    public class OverviewController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly OrderCleanupService _orderCleanupService;

        public OverviewController(ApplicationDbContext applicationDbContext, OrderCleanupService orderCleanupService)
        {
            _applicationDbContext = applicationDbContext;
            _orderCleanupService = orderCleanupService;
        }

        public IActionResult Index()
        {
            var userIdFromSession = HttpContext.Session.GetString("userId");
            var user = _applicationDbContext.ApplicationUsers.FirstOrDefault(x => x.Id == Guid.Parse(HttpContext.Session.GetString("userId")));
            var checker = _applicationDbContext.UserProductStates.FirstOrDefault(ups => ups.ApplicationUserId == user.Id);
            var viewModel = new OverviewIndexViewModel
            {
                Checker = checker != null
            };
            return View(viewModel);
        }

        public async Task<IActionResult> ShowAllOrders()
        {
            var userIdFromSession = HttpContext.Session.GetString("userId");
            _orderCleanupService.CleanupTemporaryOrders();
            var user = _applicationDbContext.ApplicationUsers.FirstOrDefault(x => x.Id == Guid.Parse(HttpContext.Session.GetString("userId")));
            var orders = await _applicationDbContext.Orders
                               .Where(o => o.ApplicationUserId == user.Id)
                               .OrderByDescending(o => o.Created)
                               .Select(o => new OrderViewModel
                               {
                                   OrderId = o.Id,
                                   Created = o.Created,
                                   OrderType = o.OrderType.Name,
                                   IsDelivered = o.DateDelivered != DateTime.MinValue,
                                   Status = o.DateDelivered != DateTime.MinValue ? "Gelevered" : "Niet gelevered"
                               }).ToListAsync();
            return View(orders);
        }

        public async Task<IActionResult> OrderInfo(Guid orderId)
        {
            _orderCleanupService.CleanupTemporaryOrders();
            var orderDetails = await _applicationDbContext.UserProductStates
                .Where(ups => ups.OrderId == orderId && ups.TransactionType != "Start")
                .OrderByDescending(ups => ups.Created)
                .Join(_applicationDbContext.OrderItems,
                      ups => new { ups.OrderId, ups.ProductId },
                       oi => new { oi.OrderId, oi.ProductId },
                      (ups, oi) => new UserProductStateViewModel
                      {
                          Name = _applicationDbContext.Products.Where(p => p.Id == ups.ProductId).FirstOrDefault().Name,
                          PhysicalStock = ups.PhysicalStock,
                          FictionalStock = ups.FictionalStock,
                          SoonAvailableStock = ups.SoonAvailableStock,
                          ReservedStock = ups.ReservedStock,
                          MinimumStock = ups.MinimumStock,
                          MaximumStock = ups.MaximumStock,
                          QuantityOrdered = oi.Quantity,
                          Status = ups.TransactionType == "Geleverd" ? "Delivered" : "Not Delivered",
                          Date = oi.Order.Created
                      }).ToListAsync();

            var filteredOrderDetails = orderDetails.Any(ups => ups.Status == "Delivered")
                ? orderDetails.Where(ups => ups.Status == "Delivered").ToList()
                : orderDetails.ToList();
            if (orderDetails == null || !orderDetails.Any())
            {
                return NotFound("Bestelgegevens niet gevonden.");
            }
            return View(filteredOrderDetails);
        }

        public IActionResult InitStartSituation()
        {
            var userIdFromSession = HttpContext.Session.GetString("userId");
            var user = _applicationDbContext.ApplicationUsers.FirstOrDefault(x => x.Id == Guid.Parse(HttpContext.Session.GetString("userId")));
            var userProductStates = _applicationDbContext.Products.Select(p => new UserProductState
            {
                Id = Guid.NewGuid(),
                ProductId = p.Id,
                ApplicationUserId = user.Id,
                Name = $"{user.UserName}{p.Name}",
                PhysicalStock = p.InitialStock,
                FictionalStock = p.InitialStock,
                SoonAvailableStock = 0,
                ReservedStock = 0,
                MinimumStock = p.InitialMinimumStock,
                MaximumStock = p.InitialMaximumStock,
                TransactionType = "Start",
                Created = DateTime.Now,
                OrderId = Guid.Empty,
                Quantity = 0
            }).ToList();
            _applicationDbContext.UserProductStates.AddRange(userProductStates);
            _applicationDbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult OrderOverview()
        {
            return View();
        }
    }
}