using Microsoft.AspNetCore.Mvc;

namespace CompanyManagement.Controllers
{
    public class AdminDashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
