using Microsoft.AspNetCore.Identity;

namespace Scala.StockSimulation.Core.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public DateTime Deleted { get; set; }
        public ICollection<UserProductState> UserProductStates { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}