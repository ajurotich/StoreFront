using System;
using System.Collections.Generic;

namespace StoreFront.Data.EF.Models;

public partial class Block
{
    public int BlockId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int? CategoryId { get; set; }

    public bool IsRenewable { get; set; }

    public bool IsStackable { get; set; }

    public int? StackSize { get; set; }

    public string ProperTool { get; set; } = null!;

    public bool Luminous { get; set; }

    public int? LightLevel { get; set; }

    public bool Transparent { get; set; }

    public bool? IsWaterloggable { get; set; }

    public bool Flammable { get; set; }

    public int? SourceId { get; set; }

    public string? Image { get; set; }

    public int? RelatedBlockId { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<Block> InverseRelatedBlock { get; set; } = new List<Block>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual Block? RelatedBlock { get; set; }

    public virtual Source? Source { get; set; }
}
