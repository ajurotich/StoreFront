using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StoreFront.Data.EF.Models;

namespace StoreFront.UI.MVC.Controllers {

	[Authorize(Roles = "admin")]
	public class ProductsController : Controller {
		private readonly McstoreContext _context;

		public ProductsController(McstoreContext context) {
			_context = context;
		}

		// GET: Products
		[AllowAnonymous]
		public async Task<IActionResult> Index() {
			var products = _context.Products
				.Where(p => !p.IsDiscontinued)
				.Include(p => p.Block)
				.Include(p => p.Block.Source)
				.Include(p => p.Block.Category);

			return View(await products.ToListAsync());
		}

		[AllowAnonymous]
		public PartialViewResult DetailsPartial(int id) {
			Product? product = _context.Products.Find(id);
			Block? block = _context.Blocks.Find(product.BlockId);
			//if(block != null) {
			//	Category? category = _context.Categories.Find(block.CategoryId);
			//	Source? source = _context.Sources.Find(block.SourceId);
			//}

			#region product/block debugging
			/*
			Console.WriteLine("\n\n------\n");

			Console.WriteLine($"Partial has begun loading with ProductId: [{id}]");

			if(product == null)
				Console.WriteLine("Product could not be found");
			else {
				Console.WriteLine(">> Product found! ");
				Console.WriteLine($"  - BlockId: [{product.BlockId}]");
				Console.WriteLine();

				if(block == null)
					Console.WriteLine("Block could not be found");
				else
					Console.WriteLine($"Block found: [{block.Name}]");
			}

			Console.WriteLine($"\n-----\n\n");
			*/
			#endregion

			return PartialView(product);
		}


		// GET: Products/Details/5
		[AllowAnonymous]
		public async Task<IActionResult> Details(int? id) {
			if(id == null) {
				return NotFound();
			}

			var product = await _context.Products
				.Include(p => p.Block)
				.FirstOrDefaultAsync(m => m.ProductId == id);
			if(product == null) {
				return NotFound();
			}

			return View(product);
		}

		// GET: Products/Create
		public IActionResult Create() {
			ViewData["BlockId"] = new SelectList(_context.Blocks, "BlockId", "Name");
			return View();
		}

		// POST: Products/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("ProductId,BlockId,Price,IsStock,CurrentStock,IsDiscontinued,StockOnOrder")] Product product) {
			if(ModelState.IsValid) {
				_context.Add(product);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			ViewData["BlockId"] = new SelectList(_context.Blocks, "BlockId", "Name", product.BlockId);
			return View(product);
		}

		// GET: Products/Edit/5
		public async Task<IActionResult> Edit(int? id) {
			if(id == null) {
				return NotFound();
			}

			var product = await _context.Products.FindAsync(id);
			if(product == null) {
				return NotFound();
			}
			ViewData["BlockId"] = new SelectList(_context.Blocks, "BlockId", "Name", product.BlockId);
			return View(product);
		}

		// POST: Products/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("ProductId,BlockId,Price,IsStock,CurrentStock,IsDiscontinued,StockOnOrder")] Product product) {
			if(id != product.ProductId) {
				return NotFound();
			}

			if(ModelState.IsValid) {
				try {
					_context.Update(product);
					await _context.SaveChangesAsync();
				}
				catch(DbUpdateConcurrencyException) {
					if(!ProductExists(product.ProductId)) {
						return NotFound();
					}
					else {
						throw;
					}
				}
				return RedirectToAction(nameof(Index));
			}
			ViewData["BlockId"] = new SelectList(_context.Blocks, "BlockId", "Name", product.BlockId);
			return View(product);
		}

		// GET: Products/Delete/5
		public async Task<IActionResult> Delete(int? id) {
			if(id == null) {
				return NotFound();
			}

			var product = await _context.Products
				.Include(p => p.Block)
				.FirstOrDefaultAsync(m => m.ProductId == id);
			if(product == null) {
				return NotFound();
			}

			return View(product);
		}

		// POST: Products/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id) {
			var product = await _context.Products.FindAsync(id);
			if(product != null) {
				_context.Products.Remove(product);
			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool ProductExists(int id) {
			return _context.Products.Any(e => e.ProductId == id);
		}
	}
}
