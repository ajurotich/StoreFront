using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace StoreFront.Data.EF;

public class BlockMetadata {

	[Required]
	[StringLength(20)]
	public string Name { get; set; } = null!;

	[DisplayFormat(NullDisplayText = "No description")]
	[StringLength(200)]
	[DataType(DataType.MultilineText)]
	public string? Description { get; set; }

	[Required]
	[Display(Name = "Renewable?")]
	public bool IsRenewable { get; set; }

	[Required]
	[Display(Name = "Stackable?")]
	public bool IsStackable { get; set; }

	[DisplayFormat(NullDisplayText = "N/A")]
	[Display(Name = "Stack Size")]
	public int? StackSize { get; set; }

	[Required]
	[Display(Name = "Proper Tool")]
	[StringLength(20)]
	public string ProperTool { get; set; } = null!;

	[Required]
	public bool Luminous { get; set; }

	[DisplayFormat(NullDisplayText = "N/A")]
	[Display(Name = "Light Level")]
	public int? LightLevel { get; set; }

	[Required]
	public bool Transparent { get; set; }

	[DisplayFormat(NullDisplayText = "N/A")]
	[Display(Name = "Waterloggable?")]
	public bool? IsWaterloggable { get; set; }

	[Required]
	public bool Flammable { get; set; }

	[DisplayFormat(NullDisplayText = "No image provided")]
	public string? Image { get; set; }

}

public class CategoryMetadata {

	[Required]
	[StringLength(20)]
	public string Name { get; set; } = null!;

	[DisplayFormat(NullDisplayText = "No description")]
	[StringLength(200)]
	public string? Description { get; set; }

}

public class OrderMetadata {

	[DisplayFormat(NullDisplayText = "Not provided", 
		DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
	[Display(Name = "Order Date")]
	public DateTime? DateOrdered { get; set; }

	[Required]
	[DisplayFormat(NullDisplayText = "Same as Buyer's Location")]
	[Display(Name = "Delivery Coordinates?")]
	[StringLength(20)]
	public string DeliveryCoords { get; set; } = null!;

}

public class OrderProductMetadata {

	[Required]
	[Range(0, int.MaxValue)]
	public int Quantity { get; set; }

}

public class ProductMetadata {

	[Required]
	[DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = true)]
	public decimal Price { get; set; }

	[Required]
	[Display(Name = "In Stock?")]
	public bool IsStock { get; set; }

	[Required]
	[Display(Name = "Current Stock")]
	[Range(0, int.MaxValue)]
	public int CurrentStock { get; set; }

	[Required]
	[Display(Name = "Discontinued?")]
	public bool IsDiscontinued { get; set; }

	[Required]
	[Display(Name = "Stock On Order")]
	[Range(0, int.MaxValue)]
	public int StockOnOrder { get; set; }

}

public class SourceMetadata {

	[Required]
	[StringLength(20)]
	public string Name { get; set; } = null!;

	[DisplayFormat(NullDisplayText = "No description")]
	[StringLength(200)]
	public string? Description { get; set; }

}

public class UserMetadata {

	[Required]
	[StringLength(100)]
	public string Name { get; set; } = null!;

	[DisplayFormat(NullDisplayText = "No coordinates provided")]
	[StringLength(20)]
	public string? HouseCoords { get; set; }

}
