using System;
using System.Collections.Generic;

namespace StoreFront.Data.EF.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public string BuyerId { get; set; } = null!;

    public DateTime? DateOrdered { get; set; }

    public string DeliveryCoords { get; set; } = null!;

    public virtual User Buyer { get; set; } = null!;

    public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();
}
