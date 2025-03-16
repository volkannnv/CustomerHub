using Microsoft.AspNetCore.Mvc;

namespace CustomerHub.Controllers
{
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}

//Home index sayfasını da HomeController'ı da kullanmıyorum, silsem de olur açıkçası
