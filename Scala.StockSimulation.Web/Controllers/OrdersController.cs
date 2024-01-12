using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scala.StockSimulation.Core.Entities;
using Scala.StockSimulation.Web.Data;
using Scala.StockSimulation.Web.Services;
using Scala.StockSimulation.Web.ViewModels;
using System.Text.Json;

namespace Scala.StockSimulation.Web.Controllers
{
    [Authorize(Roles = "Admin, Student")]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly FormHelperService _formHelperService;
        private readonly OrderCleanupService _orderCleanupService;

        public OrdersController(ApplicationDbContext applicationDbContext, FormHelperService formHelperService, OrderCleanupService orderCleanupService)
        {
            _applicationDbContext = applicationDbContext;
            _formHelperService = formHelperService;
            _orderCleanupService = orderCleanupService;
        }

        [HttpGet]
        public async Task<IActionResult> SelectProductsSupplier()
        {
            SelectProductsSupplierViewModel selectProductsSupplierViewModel = new();
            selectProductsSupplierViewModel.Supplier = await _formHelperService.GetSuppliers();
            return View(selectProductsSupplierViewModel);
        }

        public IActionResult SelectOrderType()
        {
            var errorMessage = HttpContext.Session.GetString("ErrorMessage");
            if (!string.IsNullOrEmpty(errorMessage))
            {
                ModelState.AddModelError(string.Empty, errorMessage);
                HttpContext.Session.Remove("ErrorMessage"); // Clear the message after use
            }
            var userIdFromSession = HttpContext.Session.GetString("userId");
            var selectOrderTypeParentViewModel = new SelectOrderTypeParentViewModel
            {
                OrderTypes = _applicationDbContext.OrderTypes.Select(x => new SelectOrderTypeViewModel
                {
                    Id = x.Id,
                    Name = x.Name
                })
            };
            return View(selectOrderTypeParentViewModel);
        }

        [HttpPost]
        public IActionResult SelectOrderType(Guid SelectedOrderTypeId)
        {
            HttpContext.Session.SetString("OrderTypeId", SelectedOrderTypeId.ToString());
            return RedirectToAction("SelectProductsSupplier");
        }

        [HttpPost]
        public async Task<IActionResult> SelectProductsSupplier(SelectProductsSupplierViewModel model)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("DisplayProducts", new { supplierId = model.SupplierId, articleNumber = model.ArticleNumber });
            }

            model.Supplier = await _formHelperService.GetSuppliers();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> DisplayProducts(Guid supplierId)
        {
            if (supplierId != Guid.Empty)
            {
                DisplayProductsViewModel viewModel = new()
                {
                    SupplierId = supplierId,
                    Products = await _formHelperService.GetProductsBySupplier(supplierId)
                };
                return View(viewModel);
            }
            else
            {
                return RedirectToAction("SelectOrderType");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserProductState(DisplayProductsViewModel viewModel)
        {
            try
            {
                var orderTypeId = HttpContext.Session.GetString("OrderTypeId");
                var user = _applicationDbContext.ApplicationUsers.FirstOrDefault(x => x.Id == Guid.Parse(HttpContext.Session.GetString("userId")));
                var productIds = viewModel.Products.Where(x => x.IsSelected).Select(x => x.ProductId).ToList();
                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    Created = DateTime.Now,
                    OrderTypeId = Guid.Parse(orderTypeId),
                    DateDelivered = DateTime.MinValue,
                    ApplicationUserId = user.Id,
                    IsTemporary = true
                };
                var orderTypeName = _applicationDbContext.OrderTypes.Where(x => x.Id == order.OrderTypeId).FirstOrDefault().Name;
                // Check if user already has a UserProductState for the selected products
                var checker = _applicationDbContext.UserProductStates
                    .Where(x => productIds.Contains(x.ProductId) && x.ApplicationUserId == user.Id && x.OrderId == order.Id).ToList();
                if (checker.Count == 0)
                {
                    var latestStates = new List<UserProductState>();
                    foreach (Guid productId in productIds)
                    {
                        var userProductState = _applicationDbContext.UserProductStates.Where(x => x.ProductId == productId && x.ApplicationUserId == user.Id)
                        .OrderByDescending(x => x.Created).FirstOrDefault();
                        latestStates.Add(userProductState);
                    }
                    var userStates = latestStates.Select(x => new UserProductState
                    {
                        Id = Guid.NewGuid(),
                        Name = x.Name,
                        PhysicalStock = x.PhysicalStock,
                        FictionalStock = x.FictionalStock,
                        MinimumStock = x.MinimumStock,
                        MaximumStock = x.MaximumStock,
                        SoonAvailableStock = x.SoonAvailableStock,
                        ReservedStock = x.ReservedStock,
                        ProductId = x.ProductId,
                        ApplicationUserId = user.Id,
                        Quantity = 0,
                        TransactionType = orderTypeName,
                        OrderId = order.Id,
                        Created = DateTime.Now
                    }).ToList();
                    await _applicationDbContext.UserProductStates.AddRangeAsync(userStates);
                    await _applicationDbContext.Orders.AddAsync(order);
                    await _applicationDbContext.SaveChangesAsync();
                }
                TempData["DisplayProductsViewModel"] = JsonSerializer.Serialize(viewModel);
                HttpContext.Session.SetString("OrderId", order.Id.ToString());
                return RedirectToAction("SelectedProductsSupplier", viewModel);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Overview");
            }
        }

        public IActionResult SelectedProductsSupplier()
        {
            try
            {
                var orderId = Guid.Parse(HttpContext.Session.GetString("OrderId"));
                var viewModel = JsonSerializer.Deserialize<DisplayProductsViewModel>(TempData["DisplayProductsViewModel"].ToString());
                var user = _applicationDbContext.ApplicationUsers.FirstOrDefault(x => x.Id == Guid.Parse(HttpContext.Session.GetString("userId")));
                var productIds = viewModel.Products.Where(x => x.IsSelected).Select(x => x.ProductId).ToList();
                if (productIds.Count == 0)
                {
                    HttpContext.Session.SetString("ErrorMessage", "Selecteer minimaal één product");
                    return RedirectToAction("DisplayProducts");
                }
                var products = _applicationDbContext.Products
                .Where(x => productIds.Contains(x.Id))
                .Select(x => new ProductViewModel
                {
                    ProductId = x.Id,
                    ProductName = x.Name,
                    ArticleNumber = x.ArticleNumber
                })
                .ToList();
                ViewBag.SupplierName = _applicationDbContext.Products
                    .Include(x => x.Supplier)
                    .Where(x => productIds.Contains(x.Id))
                    .Select(x => x.Supplier.Name)
                    .FirstOrDefault();
                var selectedProductsSupplierViewModel = new SelectedProductsSupplierViewModel
                {
                    UserProductState = _applicationDbContext.UserProductStates
                        .Where(x => productIds.Contains(x.ProductId) && x.ApplicationUserId == user.Id && x.OrderId == orderId)
                        .Select(x => new UserProductStateViewModel
                        {
                            Id = x.Id,
                            Name = _applicationDbContext.Products.Where(p => p.Id == x.ProductId).FirstOrDefault().Name,
                            PhysicalStock = x.PhysicalStock,
                            FictionalStock = x.FictionalStock,
                            MinimumStock = x.MinimumStock,
                            MaximumStock = x.MaximumStock,
                            SoonAvailableStock = x.SoonAvailableStock,
                            ReservedStock = x.ReservedStock,
                            OrderItem = new OrderItemViewModel()
                            {
                                ProductId = x.ProductId,
                                Quantity = 1
                            }
                        }).ToList(),
                };
                TempData["SelectedProductsSupplierViewModel"] = JsonSerializer.Serialize(selectedProductsSupplierViewModel);
                return View(selectedProductsSupplierViewModel);
            }
            catch
            {
                return RedirectToAction("Index", "Overview");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ProductsDelivered(Guid orderId)
        {
            var userIdFromSession = HttpContext.Session.GetString("userId");
            _orderCleanupService.CleanupTemporaryOrders();
            var order = await _applicationDbContext.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound("Bestelling niet gevonden.");
            }

            if (order.DateDelivered != DateTime.MinValue)
            {
                return BadRequest("Bestelling is al geleverd.");
            }

            order.DateDelivered = DateTime.Now;

            var orderType = await _applicationDbContext.OrderTypes
                .Where(ot => ot.Id == order.OrderTypeId)
                .OrderByDescending(x => x.Created)
                .FirstOrDefaultAsync();

            if (orderType == null)
            {
                return NotFound("Bestellingstype niet gevonden.");
            }

            foreach (var item in order.OrderItems)
            {
                var ups = await _applicationDbContext.UserProductStates
                    .Where(ups => ups.ProductId == item.ProductId && ups.ApplicationUserId == order.ApplicationUserId)
                    .OrderByDescending(x => x.Created)
                    .FirstOrDefaultAsync();

                if (ups != null)
                {
                    var delivery = new UserProductState
                    {
                        Id = Guid.NewGuid(),
                        Created = DateTime.Now,
                        Updated = DateTime.Now,
                        Deleted = DateTime.MinValue,
                        Name = ups.Name,
                        MinimumStock = ups.MinimumStock,
                        MaximumStock = ups.MaximumStock,
                        FictionalStock = ups.FictionalStock,
                        TransactionType = "Geleverd",
                        Quantity = item.Quantity,
                        OrderId = orderId,
                        ApplicationUserId = ups.ApplicationUserId,
                        ProductId = ups.ProductId
                    };

                    if (orderType.Name == "Klant")
                    {
                        delivery.PhysicalStock = ups.PhysicalStock - item.Quantity;
                        delivery.ReservedStock = ups.ReservedStock - item.Quantity;
                        delivery.SoonAvailableStock = ups.SoonAvailableStock;
                    }
                    else
                    {
                        delivery.PhysicalStock = ups.PhysicalStock + item.Quantity;
                        delivery.SoonAvailableStock = ups.SoonAvailableStock - item.Quantity;
                        delivery.ReservedStock = ups.ReservedStock;
                    }

                    _applicationDbContext.UserProductStates.Add(delivery);
                }
            }
            await _applicationDbContext.SaveChangesAsync();
            return RedirectToAction("ShowAllOrders", "Overview");
        }

        public async Task<IActionResult> PlaceOrder(SelectedProductsSupplierViewModel selectedQuantity)
        {
            try
            {
                var orderId = Guid.Parse(HttpContext.Session.GetString("OrderId"));
                var viewModel = JsonSerializer.Deserialize<SelectedProductsSupplierViewModel>(TempData["SelectedProductsSupplierViewModel"].ToString());
                var user = _applicationDbContext.ApplicationUsers.FirstOrDefault(x => x.Id == Guid.Parse(HttpContext.Session.GetString("userId")));
                var order = _applicationDbContext.Orders.Where(x => x.Id == orderId).FirstOrDefault();
                foreach (var item in selectedQuantity.UserProductState)
                {
                    var orderType = _applicationDbContext.OrderTypes.Where(x => x.Id == order.OrderTypeId).FirstOrDefault();
                    if (item.OrderItem.Quantity > 0 && (orderType.Name == "Klant"))
                    {
                        var orderItem = new OrderItem
                        {
                            Id = Guid.NewGuid(),
                            OrderId = order.Id,
                            ProductId = item.OrderItem.ProductId,
                            Quantity = item.OrderItem.Quantity,
                            Created = DateTime.Now,
                        };
                        _applicationDbContext.UserProductStates.Where(x => x.Id == item.Id).FirstOrDefault().Quantity += item.OrderItem.Quantity;
                        _applicationDbContext.UserProductStates.Where(x => x.Id == item.Id).FirstOrDefault().ReservedStock += item.OrderItem.Quantity;
                        _applicationDbContext.UserProductStates.Where(x => x.Id == item.Id).FirstOrDefault().FictionalStock = _applicationDbContext.UserProductStates.Where(x => x.Id == item.Id).FirstOrDefault().PhysicalStock + (_applicationDbContext.UserProductStates.Where(x => x.Id == item.Id).FirstOrDefault().SoonAvailableStock - _applicationDbContext.UserProductStates.Where(x => x.Id == item.Id).FirstOrDefault().ReservedStock);
                        await _applicationDbContext.OrderItems.AddAsync(orderItem);
                    }
                    else if (item.OrderItem.Quantity > 0 && (orderType.Name == "Leverancier"))
                    {
                        var orderItem = new OrderItem
                        {
                            Id = Guid.NewGuid(),
                            OrderId = order.Id,
                            ProductId = item.OrderItem.ProductId,
                            Quantity = item.OrderItem.Quantity,
                            Created = DateTime.Now,
                        };
                        _applicationDbContext.UserProductStates.Where(x => x.Id == item.Id).FirstOrDefault().Quantity += item.OrderItem.Quantity;
                        _applicationDbContext.UserProductStates.Where(x => x.Id == item.Id).FirstOrDefault().SoonAvailableStock += item.OrderItem.Quantity;
                        _applicationDbContext.UserProductStates.Where(x => x.Id == item.Id).FirstOrDefault().FictionalStock = _applicationDbContext.UserProductStates.Where(x => x.Id == item.Id).FirstOrDefault().PhysicalStock + (_applicationDbContext.UserProductStates.Where(x => x.Id == item.Id).FirstOrDefault().SoonAvailableStock - _applicationDbContext.UserProductStates.Where(x => x.Id == item.Id).FirstOrDefault().ReservedStock);
                        await _applicationDbContext.OrderItems.AddAsync(orderItem);
                    }
                    else
                    {
                        _orderCleanupService.CleanupTemporaryOrders();
                        ModelState.AddModelError("", "Voer een geldig aantal in");
                        return RedirectToAction("Index", "Overview");
                    }
                    order.IsTemporary = false;
                    _applicationDbContext.Orders.Update(order);
                }
                await _applicationDbContext.SaveChangesAsync();
                _orderCleanupService.CleanupTemporaryOrders();
                TempData["OrderId"] = JsonSerializer.Serialize(order.Id);
                return RedirectToAction("ShowOrderOverview");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Overview");
            }
        }

        [HttpGet]
        public IActionResult ShowOrderOverview()
        {
            try
            {
                var orderId = JsonSerializer.Deserialize<Guid>(TempData["OrderId"].ToString());
                var user = _applicationDbContext.ApplicationUsers.FirstOrDefault(x => x.Id == Guid.Parse(HttpContext.Session.GetString("userId")));
                var order = _applicationDbContext.Orders.Where(x => x.Id == orderId).FirstOrDefault();
                var OrderOverviewViewModel = new OrderOverviewViewModel
                {
                    UserProductState = _applicationDbContext.UserProductStates
                        .Where(x => x.ApplicationUserId == user.Id && x.OrderId == orderId)
                        .Select(x => new UserStateOverviewViewModel
                        {
                            Id = x.Id,
                            Name = _applicationDbContext.Products.Where(p => p.Id == x.ProductId).FirstOrDefault().Name,
                            PhysicalStock = x.PhysicalStock,
                            FictionalStock = x.FictionalStock,
                            MinimumStock = x.MinimumStock,
                            MaximumStock = x.MaximumStock,
                            SoonAvailableStock = x.SoonAvailableStock,
                            ReservedStock = x.ReservedStock,
                            Quantity = x.Quantity,
                            Product = _applicationDbContext.Products.Where(p => p.Id == x.ProductId).Select(p => p.Name).FirstOrDefault()
                        }),
                    OrderType = _applicationDbContext.OrderTypes.Where(x => x.Id == order.OrderTypeId).Select(x => x.Name).FirstOrDefault()
                };
                return View(OrderOverviewViewModel);
            }
            catch (NullReferenceException ex)
            {
                return RedirectToAction("Index", "Overview");
            }
        }

        public async Task<IActionResult> ProductsDeliveredOverview()
        {
            try
            {
                var user = _applicationDbContext.ApplicationUsers.FirstOrDefault(x => x.Id == Guid.Parse(HttpContext.Session.GetString("userId")));
                var orderDetails = await _applicationDbContext.UserProductStates
                .Where(x => x.ApplicationUserId == user.Id && x.TransactionType != "Start")
                .Join(_applicationDbContext.OrderItems,
                      ups => new { ups.ProductId, ups.OrderId },
                      oi => new { oi.ProductId, oi.OrderId },
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
                      }).Distinct().OrderByDescending(x => x.Date).ThenByDescending(x => x.Status == "Delivered")
                .ToListAsync();
                return View(orderDetails);
            }
            catch
            {
                return RedirectToAction("Login", "Account");
            }
        }
    }
}