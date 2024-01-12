namespace Scala.StockSimulation.Web.Services.Interfaces
{
    public interface IAccountService
    {
        Task<bool> Login(string username, string password);
    }
}