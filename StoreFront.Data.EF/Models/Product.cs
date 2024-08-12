using System;
using System.Collections.Generic;

namespace StoreFront.Data.EF.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public int BlockId { get; set; }

    public decimal Price { get; set; }

    public bool IsStock { get; set; }

    public int CurrentStock { get; set; }

    public bool IsDiscontinued { get; set; }

    public int StockOnOrder { get; set; }

    public virtual Block Block { get; set; } = null!;

    public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();
}
