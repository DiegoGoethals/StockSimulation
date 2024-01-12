using ExcelDataReader;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using MimeKit.Text;
using Scala.StockSimulation.Core.Entities;
using Scala.StockSimulation.Web.Data;
using Scala.StockSimulation.Web.Services;
using Scala.StockSimulation.Web.Services.Interfaces;
using Scala.StockSimulation.Web.ViewModels;
using System.Data;

namespace Scala.StockSimulation.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly LinkGenerator _linkGenerator;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly OrderCleanupService _orderCleanupService;

        public AccountController(IAccountService accountService, ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, LinkGenerator linkGenerator, SignInManager<ApplicationUser> signInManager, OrderCleanupService orderCleanupService)
        {
            _accountService = accountService;
            _applicationDbContext = applicationDbContext;
            _userManager = userManager;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _linkGenerator = linkGenerator;
            _signInManager = signInManager;
            _orderCleanupService = orderCleanupService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AccountLoginViewModel accountLoginViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(accountLoginViewModel);
            }
            var result = await _signInManager.PasswordSignInAsync(accountLoginViewModel.Email,
            accountLoginViewModel.Password, false, true);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Wrong credentials!");
                return View(accountLoginViewModel);
            }
            var user = await _applicationDbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.UserName.Equals(accountLoginViewModel.Email));
            var userRole = await _applicationDbContext.UserRoles.FirstOrDefaultAsync(ur => ur.UserId.Equals(user.Id));
            var role = await _applicationDbContext.Roles.FirstOrDefaultAsync(r => r.Id.Equals(userRole.RoleId));
            if (role.Name == "Admin")
            {
                HttpContext.Session.SetString("userId", user.Id.ToString());
                return RedirectToAction("Index", "Admin", new { area = "Admin" });
            }
            HttpContext.Session.SetString("userId", user.Id.ToString());
            return RedirectToAction("Index", "Overview");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(AccountRegisterViewModel accountRegisterViewModel)
        {
            //errors from form validation
            if (!ModelState.IsValid)
            {
                return View(accountRegisterViewModel);
            }
            if (await _userManager.FindByNameAsync(accountRegisterViewModel.Email) == null)
            {
                var user = new ApplicationUser
                {
                    UserName = accountRegisterViewModel.Email,
                    Email = accountRegisterViewModel.Email,
                    Firstname = accountRegisterViewModel.Firstname,
                    Lastname = accountRegisterViewModel.Lastname,
                    EmailConfirmed = false
                };
                var registration = await _userManager.CreateAsync(user, accountRegisterViewModel.Password);
                if (registration.Succeeded)
                {
                    user = await _userManager.FindByNameAsync(accountRegisterViewModel.Email);
                    await _userManager.AddToRoleAsync(user, "Student");
                }
                await SendValidationEmail(user);
            }
            else
            {
                ModelState.AddModelError("Email", "This email is already taken!");
            }
            return View(accountRegisterViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        private async Task<IActionResult> SendValidationEmail(ApplicationUser user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = _linkGenerator.GetUriByAction
            (
                action: "ValidateEmail",
                controller: "Account",
                scheme: _httpContextAccessor.HttpContext.Request.Scheme,
                host: _httpContextAccessor.HttpContext.Request.Host,
                values: new { userId = user.Id, token }
            );
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_configuration["Email:Account"]));
            email.To.Add(MailboxAddress.Parse(user.Email));
            email.Subject = "Confirm your emailaddress";
            email.Body = new TextPart(TextFormat.Html)
            {
                Text = $"<h5>Please confirm your emailadress</h5>" +
                $"<p>Please confirm your emailadress by clicking " +
                $"<a href='{confirmationLink}'>here</a>",
            };
            using var smtpClient = new SmtpClient();
            await smtpClient.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            await smtpClient.AuthenticateAsync(_configuration["Email:Account"], _configuration["Email:ApiKey"]);
            var result = await smtpClient.SendAsync(email);
            await smtpClient.DisconnectAsync(true);
            return RedirectToAction("login");
        }

        [HttpGet]
        public async Task<IActionResult> ValidateEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            if (await _userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, "EmailConfirmation", token))
            {
                await _userManager.ConfirmEmailAsync(user, token);
            }
            return View();
        }

        [HttpGet]
        public IActionResult RegisterImport()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterImport(IFormFile file)
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
                    var email = GetColumnValue(reader, columnNames, "Firstname") + "." + GetColumnValue(reader, columnNames, "Lastname").Replace(" ", "") + "@scala.be";
                    var user = await _userManager.FindByNameAsync(email);
                    if (user == null)
                    {
                        user = new ApplicationUser
                        {
                            UserName = email,
                            Email = email,
                            Firstname = GetColumnValue(reader, columnNames, "Firstname"),
                            Lastname = GetColumnValue(reader, columnNames, "Lastname")
                        };
                        var result = await _userManager.CreateAsync(user, GetColumnValue(reader, columnNames, "Password"));
                        if (result.Succeeded)
                        {
                            user = await _userManager.FindByNameAsync(email);
                            await _userManager.AddToRoleAsync(user, GetColumnValue(reader, columnNames, "Role"));
                        }
                    }
                    else
                    {
                        await _userManager.RemoveFromRoleAsync(user, GetColumnValue(reader, columnNames, "Role"));
                        await _userManager.DeleteAsync(user);
                        var result = await _userManager.CreateAsync(user, GetColumnValue(reader, columnNames, "Password"));
                        if (result.Succeeded)
                        {
                            user = await _userManager.FindByNameAsync(email);
                            await _userManager.AddToRoleAsync(user, GetColumnValue(reader, columnNames, "Role"));
                        }
                    }
                }
                ModelState.AddModelError("", "Importeren is gelukt");
            }
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

        public async Task<IActionResult> Logout()
        {
            _orderCleanupService.CleanupTemporaryOrders();
            HttpContext.Session.Remove("userId");
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}