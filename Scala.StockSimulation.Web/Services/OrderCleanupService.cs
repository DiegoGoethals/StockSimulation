using Scala.StockSimulation.Web.Data;

namespace Scala.StockSimulation.Web.Services
{
    public class OrderCleanupService
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public OrderCleanupService(ApplicationDbContext context)
        {
            _applicationDbContext = context;
        }

        public void CleanupTemporaryOrders()
        {
            var cutoffDate = DateTime.Now;

            var incompleteOrders = _applicationDbContext.Orders
                .Where(o => o.IsTemporary && o.Created < cutoffDate)
                .ToList();

            if (incompleteOrders.Count == 0)
            {
                return;
            }

            var incompleteUserProductStates = _applicationDbContext.UserProductStates.AsEnumerable()
                .Where(ups => incompleteOrders.Any(o => o.Id == ups.OrderId))
                .ToList();

            _applicationDbContext.UserProductStates.RemoveRange(incompleteUserProductStates);
            _applicationDbContext.Orders.RemoveRange(incompleteOrders);
            _applicationDbContext.SaveChanges();
        }
    }
}