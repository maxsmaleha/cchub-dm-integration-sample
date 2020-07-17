using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DocketManagerSample.Models.Home;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

namespace DocketManagerSample.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IIdentityServerInteractionService interaction, IWebHostEnvironment environment, IConfiguration configuration)
        {
            _logger = logger;
            _interaction = interaction;
            _environment = environment;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult BackOffice()
        {
            /*
             * Here we want to display a BackOffice in <iframe> element. We try to sign in to a tenant and if it does not exist, 
             * we just create it.
             * To identity tenants, we use a so-called TenancyName parameter. It can be any URL-friendly string, however, we 
             * recommend building it based on the domain name of your customer store (we do it for Shopify).
             * 
             * For this example, we will hardcode a TenancyName, but in a real-life application, you are going to determine it
             * based on the account of a customer.
             */
            var storeUrl = HttpUtility.UrlEncode(_configuration["BackOffice:TenantName"]);
            var model = new BackOfficeViewModel { LoginUrl = $"{_configuration["BackOffice:FrontendUrl"]}account/login/docket-manager?shop={storeUrl}" };

            return View("BackOffice", model);
        }

        public IActionResult ViewProducts()
        {
            return View("View", TestProducts.Products);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Shows the error page
        /// </summary>
        public async Task<IActionResult> Error(string errorId)
        {
            var vm = new ErrorViewModel();

            // retrieve error details from identityserver
            var message = await _interaction.GetErrorContextAsync(errorId);
            if (message != null)
            {
                vm.Error = message;

                if (!_environment.IsDevelopment())
                {
                    // only show in development
                    message.ErrorDescription = null;
                }
            }

            return View("Error", vm);
        }
    }
}
