using StoreFront.UI.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using MimeKit; //Added for access to MimeMessage class
using MailKit.Net.Smtp;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization; //Added for access to SmtpClient class

namespace GadgetStore.UI.MVC.Controllers;
public class HomeController : Controller {

	private readonly ILogger<HomeController> _logger;
	private readonly IConfiguration _config;


	public HomeController(ILogger<HomeController> logger, IConfiguration configuration) {
		_logger = logger;
		_config = configuration;
	}

	public IActionResult Index() => View();

	public IActionResult Shop() => View();
	
	public IActionResult About() => View();
	
	public IActionResult Services() => View();
	
	public IActionResult Blog() => View();

	#region contact
	[Authorize]
	public IActionResult Contact() => View();

	[HttpPost]
	[Authorize]
	public IActionResult Contact(ContactViewModel cvm){
		
		if(!ModelState.IsValid) return View(cvm);
		
		string credClient = _config.GetValue<string>("Credentials:Email:Client");
		string credUser = _config.GetValue<string>("Credentials:Email:User");
		string credPassword = _config.GetValue<string>("Credentials:Email:Password");
		string credRecipient = _config.GetValue<string>("Credentials:Email:Recipient");

		string msg = $"You've got mail!\nMessage from {cvm.Name} using {cvm.Email}.\n\n" +
			$"Subject: {cvm.Subject}\nMessage: {cvm.Message}";

		var mm = new MimeMessage();
		mm.From.Add(new MailboxAddress("No Reply", credUser));
		mm.To.Add(new MailboxAddress("You", credRecipient));
		mm.ReplyTo.Add(new MailboxAddress(cvm.Name, cvm.Email));
		mm.Subject = "MCStore Message: " + cvm.Subject;
		mm.Body = new TextPart("HTML") { Text = msg };

		using(var client = new SmtpClient()) {
			client.Connect(credClient, 465);

			client.Authenticate(credUser, credPassword);

			try { client.Send(mm); }
			catch(Exception ex) {
				ViewBag.ErrorMessage = $"Error occured while sending message: \n{ex.Message}";
				return View(cvm);
			}

		}

		return View("EmailConfirmation", cvm);
	}

	#endregion
	
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error() =>
		View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

}
