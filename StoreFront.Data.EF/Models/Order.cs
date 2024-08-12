using System;
using System.Collections.Generic;

namespace StoreFront.Data.EF.Models;

public partial class Order {
	public int OrderId { get; set; }

	public int BuyerId { get; set; }

	public int SupplierId { get; set; }

	public DateTime? DateOrdered { get; set; }

	public bool DeliverToBuyerLocation { get; set; }

	public string? DeliveryCoords { get; set; }

	public virtual User Buyer { get; set; } = null!;

	public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();

	public virtual User Supplier { get; set; } = null!;
}
