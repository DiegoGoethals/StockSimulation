using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using Scala.StockSimulation.Web.Data;
using Scala.StockSimulation.Web.ViewModels;

namespace Scala.StockSimulation.Web.Services
{
    public class ExcelExportService
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public ExcelExportService(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public byte[] ExportOrderItems(string userId)
        {
            var user = _applicationDbContext.ApplicationUsers.FirstOrDefault(x => x.Id == Guid.Parse(userId));
            var userRole = _applicationDbContext.UserRoles.FirstOrDefault(ur => ur.UserId == user.Id);
            var role = _applicationDbContext.Roles.FirstOrDefault(r => r.Id == userRole.RoleId);

            if (role?.Name == "Admin")
            {
                return GenerateExcelFile(null);
            }
            else
            {
                return null;
            }
        }

        public byte[] ExportStudentOrderItems(string studentName)
        {
            if (string.IsNullOrEmpty(studentName))
            {
                return null;
            }
            return GenerateExcelFile(studentName);
        }

        private byte[] GenerateExcelFile(string studentName)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheetName = studentName == null ? "Orders" : $"Orders{studentName}";
                var worksheet = workbook.Worksheets.Add(worksheetName);

                var query = _applicationDbContext.UserProductStates
                    .Include(ups => ups.ApplicationUser)
                    .Include(ups => ups.Product)
                    .Where(ups => ups.TransactionType != "Start");

                if (studentName != null)
                {
                    query = query.Where(a => (a.ApplicationUser.Firstname + " " + a.ApplicationUser.Lastname) == studentName);
                }

                var orders = query.Select(ups => new UserProductStateViewModel
                {
                    UserName = ups.ApplicationUser.UserName,
                    Name = ups.Product.Name,
                    OrderId = ups.OrderId,
                    PhysicalStock = ups.PhysicalStock,
                    FictionalStock = ups.FictionalStock,
                    MinimumStock = ups.MinimumStock,
                    MaximumStock = ups.MaximumStock,
                    SoonAvailableStock = ups.SoonAvailableStock,
                    ReservedStock = ups.ReservedStock,
                    QuantityOrdered = ups.Quantity,
                    TransactionType = ups.TransactionType,
                    Date = ups.Created,
                    Status = ups.TransactionType == "Geleverd" ? "Geleverd" : "Niet Geleverd"
                }).OrderByDescending(ups => ups.Date).ToList();

                PopulateWorksheet(worksheet, orders);

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        private void PopulateWorksheet(IXLWorksheet worksheet, List<UserProductStateViewModel> orders)
        {
            var headerRow = new[] { "Gebruiker", "Product", "Order ID", "Fysieke stock", "Fictieve stock",
                                    "Binnenkort beschikbaar", "Gereserveerd", "MinimumStock", "MaximumStock",
                                    "Aantal besteld", "Status", "Datum", "Geleverd" };

            for (int i = 0; i < headerRow.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headerRow[i];
            }

            int currentRow = 2;
            foreach (var order in orders)
            {
                worksheet.Cell(currentRow, 1).Value = order.UserName;
                worksheet.Cell(currentRow, 2).Value = order.Name;
                worksheet.Cell(currentRow, 3).Value = order.OrderId.ToString();
                worksheet.Cell(currentRow, 4).Value = order.PhysicalStock;
                worksheet.Cell(currentRow, 5).Value = order.FictionalStock;
                worksheet.Cell(currentRow, 6).Value = order.SoonAvailableStock;
                worksheet.Cell(currentRow, 7).Value = order.ReservedStock;
                worksheet.Cell(currentRow, 8).Value = order.MinimumStock;
                worksheet.Cell(currentRow, 9).Value = order.MaximumStock;
                worksheet.Cell(currentRow, 10).Value = order.QuantityOrdered;
                worksheet.Cell(currentRow, 11).Value = order.TransactionType;
                worksheet.Cell(currentRow, 12).Value = order.Date;
                worksheet.Cell(currentRow, 13).Value = order.Status;
                currentRow++;
            }

            worksheet.Columns().AdjustToContents();
        }
    }
}