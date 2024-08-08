using StoreFront.UI.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GadgetStore.UI.MVC.Controllers;
public class HomeController : Controller {
	private readonly ILogger<HomeController> _logger;

	public HomeController(ILogger<HomeController> logger) {
		_logger = logger;
	}

	public IActionResult Index() => View();

	public IActionResult Shop() => View();
	
	public IActionResult About() => View();
	
	public IActionResult Services() => View();
	
	public IActionResult Blog() => View();
	
	public IActionResult Contact() => View();
	
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error() =>
		View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

}
