using System;
using System.Collections.Generic;

namespace StoreFront.Data.EF.Models;

public partial class User {
	public string UserId { get; set; }

	public string Name { get; set; } = null!;

	public string? HouseCoords { get; set; }

	public virtual ICollection<Order> OrderBuyers { get; set; } = new List<Order>();

	public virtual ICollection<Order> OrderSuppliers { get; set; } = new List<Order>();
}
