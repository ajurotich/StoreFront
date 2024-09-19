using System;
using System.Collections.Generic;

namespace StoreFront.Data.EF.Models;

public partial class User
{
    public string UserId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string HouseCoords { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
