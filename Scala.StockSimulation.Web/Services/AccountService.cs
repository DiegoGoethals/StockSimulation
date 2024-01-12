using Microsoft.EntityFrameworkCore;
using Scala.StockSimulation.Web.Data;
using Scala.StockSimulation.Web.Services.Interfaces;

namespace Scala.StockSimulation.Web.Services
{
    public class AccountService : IAccountService
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public AccountService(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<bool> Login(string username, string password)
        {
            var user = await _applicationDbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.UserName.Equals(username));
            if (user == null)
            {
                return false;
            }
            return true;
        }
    }
}