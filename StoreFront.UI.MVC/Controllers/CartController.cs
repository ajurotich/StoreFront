using StoreFront.Data.EF.Models;
using StoreFront.UI.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace StoreFront.UI.MVC.Controllers;

#region Steps to Implement Session Based Shopping Cart
/*
*	1) Register Session in Program.cs (builder.Services.AddSessoion & app.UseSession)
*	2) Create the CartItemViewModel class in UI MOdels folder (saving qty and product)
*	3) Add the 'Add to Cart' button in Index and/or Details view of Products
*	4) Create the CartController (empty controller)
*		- add using statements (above)
*		- add props for GadgetStoreContext & UserManager
*		- create ctor for controller > assign values to context & usermanager
*		- code AddToCart() action
*		- code Index() action
*		- code Index view
*			- start w/ basic table structure
*			- show items that are easily accessible (like props from model)
*			- calc + show line total price
*			- add RemoveFromCart() button <a>
*		- code RemoveFromCart() action
*			- verify the button for removefrom cart in the index is coded with controller/action/id
*		- Add UpdateCart <form> to index view
*		- code UpdateCart action
*		- add submit order buitton to index view
*		- code SubmitOrder() action
*/
#endregion

public class CartController : Controller {

	//PROPS
	private readonly McstoreContext _context;
	private readonly UserManager<IdentityUser> _userManager;

	//CTOR
	public CartController(McstoreContext context, UserManager<IdentityUser> userManager) {
		_context = context;
		_userManager = userManager;
	}

	//VIEWS
	public IActionResult Index() {
		//get contents from session cart > convert from json to C# obj
		// - pass collection to strongly typed view

		//get session cart contents
		string? sessionCart = HttpContext.Session.GetString("cart");

		//make shell for local c# cart
		Dictionary<int, CartItemViewModel>? cart = null;

		//check if cart was null
		if(sessionCart == null || sessionCart.Count() == 0) {
			//cart is empty (either never added anything or removed everything)
			cart = new Dictionary<int, CartItemViewModel>();

			ViewBag.CartMessage = "No items in cart";
		}
		else {
			ViewBag.CartMessage = null;

			//convert json to c# obj
			cart = JsonConvert.DeserializeObject<Dictionary<int, CartItemViewModel>>(sessionCart);
		}

		try {
			return View(cart);
		}
		catch(Exception e) {
			ViewBag.CartMessage = e.Message;
			return View();
		}
	}

	//ACTIONS
	public IActionResult AddToCart(int id) {

		//create empty shell for LOCAL cart variable
		Dictionary<int, CartItemViewModel>? cart = null;

		#region Session Notes
		/*
		*	session is a tool available on server-side that can store info for a user
		*	while they are actively using our site
		*	
		*	typically the session lasts 20 minutes (adjustable in program.cs)
		*	once 20 mins is up, session var is disposed
		*	
		*	values that we can store in session are limited to: string, int
		*		- we have to be creative when trying to store complex types
		*		- to keep info separated into props, convert C# obj into string
		*		  based data format (JSON)
		*/
		#endregion

		//get value stored in session with "cart" key
		string? sessionCart = HttpContext.Session.GetString("cart");

		//validate session object exists
		// - if its not, create new local collection
		if(string.IsNullOrEmpty(sessionCart))
			cart = new Dictionary<int, CartItemViewModel>();
		else {
			//cart exists > transfer session cart items to local cart
			// - DeserializeObject() > converts JSON to C# obj (must specify which obj (dict<int, cart>))
			cart = JsonConvert.DeserializeObject<Dictionary<int, CartItemViewModel>>(sessionCart);
		}

		//add newly selected products to cart
		// - retrieve products from DB using key
		Product? product = _context.Products
			.Include(p => p.Block)
			.FirstOrDefault(p => p.ProductId == id);
		
		

		if(product == null)
			return RedirectToAction("Index", "Products");

		#region product/block debugging
		/*
		Console.WriteLine("\n\n------\n");

		Console.WriteLine($"Partial has begun loading with ProductId: [{id}]");

		if(product == null)
			Console.WriteLine("Product could not be found");
		else {
			Console.WriteLine(">> Product found! ");
			Console.WriteLine($"  - BlockId: [{product.BlockId}]");
			Console.WriteLine();

			if(block == null)
				Console.WriteLine("Block could not be found");
			else
				Console.WriteLine($"Block found: [{block.Name}]");
		}

		Console.WriteLine($"\n-----\n\n");
		*/
		#endregion

		//instantiate the obj to add to cart
		CartItemViewModel civm = new CartItemViewModel(product, 1); //add one product

		//if product is in cart already (checked with id) then increase qty by one
		if(cart.ContainsKey(product.ProductId))
			cart[product.ProductId].Quantity++;
		else
			cart.Add(product.ProductId, civm);

		//update session cart var
		// - take local copy and serialize it into json
		// - assign value to session
		string jsonCart = JsonConvert.SerializeObject(
			cart, new JsonSerializerSettings {ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
		);
		HttpContext.Session.SetString("cart", jsonCart);

		return RedirectToAction("Index");
	}

	public IActionResult UpdateCart(int productID, int quantity) {

		//get current cart
		string? sessionCart = HttpContext.Session.GetString("cart");

		//deserialize data (json to C#)
		Dictionary<int, CartItemViewModel> cart = JsonConvert.DeserializeObject<Dictionary<int, CartItemViewModel>>(sessionCart);

		//update qty for dict key
		cart[productID].Quantity = quantity;

		//update session
		string jsonCart = JsonConvert.SerializeObject(cart);
		HttpContext.Session.SetString("cart", jsonCart);

		return RedirectToAction("Index");
	}

	public IActionResult RemoveItem(int id) {

		//get current cart
		string? sessionCart = HttpContext.Session.GetString("cart");

		//deserialize data (json to C#)
		Dictionary<int, CartItemViewModel> cart = JsonConvert.DeserializeObject<Dictionary<int, CartItemViewModel>>(sessionCart);

		//remove cart item
		cart.Remove(id);

		//if cart is empty, delete from session
		if(cart.Count() == 0) {
			HttpContext.Session.Remove("cart");
			return RedirectToAction("Index");
		}

		//else update cart
		//update session
		string jsonCart = JsonConvert.SerializeObject(cart);
		HttpContext.Session.SetString("cart", jsonCart);

		return RedirectToAction("Index");
	}

	public async Task<IActionResult> SubmitOrder() {

		#region Planning Order Submission
		/*
		*	1. Create Order obj > save to DB
		*		- orderdate (DT.now)
		*		- userid (from current user)
		*		- shiptoname/city/state/zip > from users userdetails
		*		
		*	2. Create OrderProducts obj for every item in cart (same as OrderDetails)
		*		- product id (cart)
		*		- order id (created order)
		*		- quantity (cart)
		*		- product price (cart)
		*	
		*	3. Add record to _context & save DB changes
		*/
		#endregion

		//get userid
		string? userID = (await _userManager.GetUserAsync(HttpContext.User))?.Id;

		//get userdetails
		User? userDetail = _context.Users.Find(userID);

		//make order obj + assign values
		Order o = new Order() {
			BuyerId = userID,
			DateOrdered = DateTime.Now,
			DeliveryCoords = userDetail.HouseCoords,
		};

		//add order to _context
		_context.Orders.Add(o);

		//get current cart
		string? sessionCart = HttpContext.Session.GetString("cart");

		//deserialize data (json to C#)
		Dictionary<int, CartItemViewModel> cart = JsonConvert.DeserializeObject<Dictionary<int, CartItemViewModel>>(sessionCart);

		//make orderproduct obj for each cart item
		foreach(var item in cart) {
			OrderProduct op = new OrderProduct() {
				OrderId = o.OrderId,
				ProductId = item.Key,
				Quantity = Convert.ToInt16(item.Value.Quantity),
			};

			o.OrderProducts.Add(op);
		}

		//commit and save changes to db
		_context.SaveChanges();

		return RedirectToAction("Index", "Orders"); //show orders
	}

}
