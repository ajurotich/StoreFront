using StoreFront.Data.EF.Models;

namespace StoreFront.UI.MVC.Models;

public class CartItemViewModel {

	public Product Product { get; set; }
	public int Quantity { get; set; }

	public CartItemViewModel(Product product, int qty) {
		Product = product;
		Quantity = qty;
	}

}
