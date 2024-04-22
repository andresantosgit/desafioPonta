using BNB.ProjetoReferencia.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BNB.ProjetoReferencia.WebUI.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /// <summary>
        /// Logout.
        /// </summary>
        /// <returns>Redireciona ao Home.</returns>
        public ActionResult Logout()
        {
            AutorizadorService.Logout(this.HttpContext.Request);
            return this.RedirectToAction("Index", "Home");
        }
    }
}
