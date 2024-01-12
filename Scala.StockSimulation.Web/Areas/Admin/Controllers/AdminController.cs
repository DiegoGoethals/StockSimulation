using ExcelDataReader;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scala.StockSimulation.Core.Entities;
using Scala.StockSimulation.Web.Data;
using Scala.StockSimulation.Web.Services;
using Scala.StockSimulation.Web.ViewModels;
using System.Data;

namespace Scala.StockSimulation.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly OrderCleanupService _orderCleanupService;
        private readonly ExcelExportService _excelExportService;

        public AdminController(OrderCleanupService orderCleanupService, ApplicationDbContext applicationDbContext, ExcelExportService excelExportService)
        {
            _orderCleanupService = orderCleanupService;
            _applicationDbContext = applicationDbContext;
            _excelExportService = excelExportService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ShowAllOrders(Guid applicationUserId)
        {
            try
            {
                var user = _applicationDbContext.ApplicationUsers.FirstOrDefault(x => x.Id == applicationUserId);
                var orders = await _applicationDbContext.Orders
                    .Where(o => o.ApplicationUserId == applicationUserId)
                    .Include(o => o.OrderType)
                    .ToListAsync();

                if (orders == null || orders.Count == 0)
                {
                    var studentViewModel = new StudentViewModel
                    {
                        Id = user.Id,
                        Name = $"{user.Firstname} {user.Lastname}"
                    };
                    return View("Error", studentViewModel);
                }

                var ordersViewModel = orders.Select(o => new OrderViewModel
                {
                    OrderId = o.Id,
                    Created = o.Created,
                    OrderType = o.OrderType.Name,
                    Status = o.DateDelivered == DateTime.MinValue ? "Niet gelevered" : "Gelevered",
                    UserName = user.Firstname + " " + user.Lastname,
                }).OrderByDescending(o => o.Created).ToList();

                return View("ShowAllOrders", ordersViewModel);
            }
            catch
            {
                return RedirectToAction("ShowStudents");
            }
        }

        public IActionResult ShowStudents()
        {
            var user = _applicationDbContext.ApplicationUsers.FirstOrDefault(x => x.Id == Guid.Parse(HttpContext.Session.GetString("userId")));
            var userRole = _applicationDbContext.UserRoles.FirstOrDefault(ur => ur.UserId.Equals(user.Id));
            var studentIds = _applicationDbContext.UserRoles.Where(ur => !ur.UserId.Equals(user.Id))
                .Select(Id => Id.UserId).ToList();
            var students = _applicationDbContext.ApplicationUsers.Where(u => studentIds.Contains(u.Id)).ToList();
            var showStudentsViewModel = new ShowStudentsViewModel
            {
                Students = students.Select(x => new StudentViewModel
                {
                    Id = x.Id,
                    Name = $"{x.Firstname} {x.Lastname}"
                }).ToList()
            };
            return View(showStudentsViewModel);
        }

        [HttpPost]
        public IActionResult Search(string studentToSearch)
        {
            var student = _applicationDbContext.ApplicationUsers
                .Where(s => (s.Firstname + " " + s.Lastname).Contains(studentToSearch)).FirstOrDefault();
            return RedirectToAction("ShowResults", student);
        }

        public IActionResult ShowResults(ApplicationUser student)
        {
            var showResultsViewModel = new ShowResultsViewModel
            {
                Name = $"{student.Firstname} {student.Lastname}",
                Orders = _applicationDbContext.UserProductStates
                    .Where(x => x.ApplicationUserId == student.Id && x.TransactionType != "Start")
                    .Join(_applicationDbContext.OrderItems,
                        ups => ups.ProductId,
                        oi => oi.ProductId,
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
                            Status = ups.TransactionType == "Geleverd" ? "Geleverd" : "Niet Geleverd",
                            Date = ups.Created
                        }).Distinct().OrderByDescending(x => x.Date)
                    .ToList()
            };
            return View(showResultsViewModel);
        }

        public IActionResult ResetStudent(Guid applicationUserId)
        {
            try
            {
                var userProductStates = _applicationDbContext.UserProductStates.Where(x => x.ApplicationUserId == applicationUserId).ToList();
                var orders = _applicationDbContext.Orders.Where(x => x.ApplicationUserId == applicationUserId).ToList();
                var orderItems = _applicationDbContext.OrderItems.Where(x => x.OrderId == orders.FirstOrDefault().Id).ToList();
                _applicationDbContext.OrderItems.RemoveRange(orderItems);
                _applicationDbContext.Orders.RemoveRange(orders);
                _applicationDbContext.UserProductStates.RemoveRange(userProductStates);
                _applicationDbContext.SaveChanges();
                return RedirectToAction("ShowStudents");
            }
            catch
            {
                return RedirectToAction("ShowStudents");
            }
        }

        [HttpGet]
        public IActionResult ImportStartSituation()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ImportStartSituation(IFormFile file)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            if (file != null && file.Length > 0)
            {
                if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase) ||
                    Path.GetExtension(file.FileName).Equals(".xls", StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError("", "Gelieve enkel een Excel file in te dienen");
                    return View();
                }
                _applicationDbContext.Suppliers.RemoveRange(_applicationDbContext.Suppliers);
                _applicationDbContext.OrderItems.RemoveRange(_applicationDbContext.OrderItems);
                _applicationDbContext.Orders.RemoveRange(_applicationDbContext.Orders);
                _applicationDbContext.UserProductStates.RemoveRange(_applicationDbContext.UserProductStates);
                _applicationDbContext.Products.RemoveRange(_applicationDbContext.Products);
                _applicationDbContext.SaveChanges();
                using var stream = file.OpenReadStream();

                using var reader = ExcelReaderFactory.CreateReader(stream);
                var columnNames = new List<string>();

                reader.Read();
                for (int column = 0; column < reader.FieldCount; column++)
                {
                    columnNames.Add(reader.GetValue(column)?.ToString());
                }

                while (reader.Read())
                {
                    // Check if supplier already exists if not create new supplier
                    var supplier = _applicationDbContext.Suppliers.Where(s => s.Name == GetColumnValue(reader, columnNames, "Supplier"))
                        .FirstOrDefault();
                    if (supplier == null)
                    {
                        supplier = new Supplier
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.Now,
                            Updated = DateTime.Now,
                            Deleted = DateTime.MinValue,
                            Name = GetColumnValue(reader, columnNames, "Supplier"),
                        };
                        _applicationDbContext.Suppliers.Add(supplier);
                        _applicationDbContext.SaveChanges();
                    }

                    var product = new Product
                    {
                        Id = Guid.NewGuid(),
                        Created = DateTime.Now,
                        Updated = DateTime.Now,
                        Deleted = DateTime.MinValue,
                        Name = GetColumnValue(reader, columnNames, "Name"),
                        Description = GetColumnValue(reader, columnNames, "Description"),
                        Price = Convert.ToDecimal(GetColumnValue(reader, columnNames, "Price")),
                        ArticleNumber = GetColumnValue(reader, columnNames, "ArticleNumber"),
                        InitialMinimumStock = Convert.ToInt32(GetColumnValue(reader, columnNames, "InitialMinimumStock")),
                        InitialMaximumStock = Convert.ToInt32(GetColumnValue(reader, columnNames, "InitialMaximumStock")),
                        InitialStock = Convert.ToInt32(GetColumnValue(reader, columnNames, "InitialStock")),
                        SupplierId = supplier.Id
                    };
                    _applicationDbContext.Products.Add(product);
                    _applicationDbContext.SaveChanges();
                }
            }
            ModelState.AddModelError("", "Importeren is gelukt");
            return View();
        }

        /*
            This method checks what the column name is and returns the index of that column.
            Important is that the column names are the same in every excel file for the above method to work.
        */

        private static string GetColumnValue(IDataReader reader, List<string> columnNames, string columnName)
        {
            var columnIndex = columnNames.IndexOf(columnName);
            return reader.GetValue(columnIndex)?.ToString();
        }

        public async Task<IActionResult> ShowOrderItems(Guid orderId)
        {
            try
            {
                _orderCleanupService.CleanupTemporaryOrders();
                //get the selected studentId used to get the orders
                ViewBag.userId = _applicationDbContext.ApplicationUsers
                    .Where(u => u.Id == _applicationDbContext.Orders
                        .Where(o => o.Id == orderId)
                            .FirstOrDefault().ApplicationUserId)
                                .FirstOrDefault().Id;
                var showAdminOrderDetails = await _applicationDbContext.UserProductStates
                    .Where(ups => ups.OrderId == orderId && ups.TransactionType != "Start")
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
                                Status = oi.Order.DateDelivered != DateTime.MinValue ? "Delivered" : "Not Delivered",
                                Date = oi.Order.Created,
                                OrderId = oi.OrderId,
                                UserName = _applicationDbContext.ApplicationUsers.Where(u => u.Id == ups.ApplicationUserId).FirstOrDefault()
                                .Firstname + " " + _applicationDbContext.ApplicationUsers.Where(u => u.Id == ups.ApplicationUserId).FirstOrDefault().Lastname,
                            }).OrderByDescending(ups => ups.Date)
                        .ToListAsync();
                if (showAdminOrderDetails == null || !showAdminOrderDetails.Any())
                {
                    return NotFound("Bestelgegevens niet gevonden.");
                }
                return View(showAdminOrderDetails);
            }
            catch
            {
                return RedirectToAction("ShowAllOrders");
            }
        }

        public IActionResult ShowExportOrderItems()
        {
            return View("ExportOrderItems");
        }

        public IActionResult ExportOrderItems()
        {
            try
            {
                var userId = HttpContext.Session.GetString("userId");
                var fileContent = _excelExportService.ExportOrderItems(userId);

                if (fileContent == null)
                {
                    return RedirectToAction("Index", "Overview");
                }
                return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Orders.xlsx");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        public IActionResult ExportStudentOrderItems(string studentName)
        {
            try
            {
                var content = _excelExportService.ExportStudentOrderItems(studentName);
                if (content == null)
                {
                    return View("ShowResults");
                }
                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Orders{studentName}.xlsx");
            }
            catch
            {
                return RedirectToAction("ShowResult");
            }
        }
    }
}