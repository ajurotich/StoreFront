using System;
using System.Collections.Generic;

namespace StoreFront.Data.EF.Models;

public partial class Source
{
    public int SourceId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Block> Blocks { get; set; } = new List<Block>();
}
