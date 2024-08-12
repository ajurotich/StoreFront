using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations; // adds data annotations (required, display, string length...)
using Microsoft.AspNetCore.Mvc;

namespace StoreFront.Data.EF.Models;

public partial class Partials {

	[ModelMetadataType(typeof(BlockMetadata))]
	public partial class Block { }

	[ModelMetadataType(typeof(CategoryMetadata))]
	public partial class Category { }

	[ModelMetadataType(typeof(OrderMetadata))]
	public partial class Order { }

	[ModelMetadataType(typeof(OrderProductMetadata))]
	public partial class OrderProduct { }

	[ModelMetadataType(typeof(ProductMetadata))]
	public partial class Product { }

	[ModelMetadataType(typeof(SourceMetadata))]
	public partial class Source { }

	[ModelMetadataType(typeof(UserMetadata))]
	public partial class User { }

}
