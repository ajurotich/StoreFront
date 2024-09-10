using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace StoreFront.UI.MVC.Models;

[Keyless]
public class ContactViewModel {

	[Required(ErrorMessage = "Name is required")]
	public string Name { get; set; } = null;

	[Required(ErrorMessage = "Email is required")]
	[EmailAddress(ErrorMessage = "Invalid email")]
	public string Email { get; set; } = null;

	[Required(ErrorMessage = "Subject is required")]
	public string Subject { get; set; } = null;
	
	[Required(ErrorMessage = "Message is required")]
	public string Message { get; set; } = null;

}
