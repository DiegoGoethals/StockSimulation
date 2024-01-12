using Scala.StockSimulation.Core.Entities;
using Scala.StockSimulation.Web.ViewModels;

namespace Scala.StockSimulation.Web.Areas.Admin.ViewModels
{
    public class AdminShowOrderItemViewModel : UserProductStateViewModel
    {
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}