using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StoreFront.Data.EF.Models;
using System.Drawing;
using StoreFront.UI.MVC.Utilities;
using Microsoft.AspNetCore.Authorization;

namespace StoreFront.UI.MVC.Controllers {

	[Authorize(Roles = "admin")]
	public class BlocksController : Controller {

		// FIELDS
		private readonly McstoreContext _context;
		private readonly IWebHostEnvironment _webHostEnvironment;

		// CTOR
		public BlocksController(McstoreContext context, IWebHostEnvironment webHostEnvironment) {
			_context = context;
			_webHostEnvironment = webHostEnvironment;
		}

		// GET: Blocks
		[AllowAnonymous]
		public async Task<IActionResult> Index() {
			var mcstoreContext = _context.Blocks.Include(b => b.Category).Include(b => b.RelatedBlock).Include(b => b.Source);
			return View(await mcstoreContext.ToListAsync());
		}

		// GET: Blocks/Details/5
		[AllowAnonymous]
		public async Task<IActionResult> Details(int? id) {
			if(id == null) {
				return NotFound();
			}

			var block = await _context.Blocks
				.Include(b => b.Category)
				.Include(b => b.RelatedBlock)
				.Include(b => b.Source)
				.FirstOrDefaultAsync(m => m.BlockId == id);
			if(block == null) {
				return NotFound();
			}

			return View(block);
		}

		// GET: Blocks/Create
		public IActionResult Create() {
			ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name");
			ViewData["RelatedBlockId"] = new SelectList(_context.Blocks, "BlockId", "Name");
			ViewData["SourceId"] = new SelectList(_context.Sources, "SourceId", "Name");
			return View();
		}

		// POST: Blocks/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("BlockId,Name,Description,CategoryId,IsRenewable,IsStackable,StackSize,ProperTool,Luminous,LightLevel,Transparent,IsWaterloggable,Flammable,SourceId,BlockImage,Image,RelatedBlockId")] Block block) {
			if(ModelState.IsValid) {


				#region file upload

				if(block.BlockImage != null) {

					//allowed extensions list
					string[] validExts = {
						".jpg",
						".jpeg",
						".png",
						".gif"
					};

					//check file type
					string ext = Path.GetExtension(block.BlockImage.FileName); //get the extension

					//check if file has valid extensions
					// and if size will work with .net app
					if(validExts.Contains(ext.ToLower()) && block.BlockImage.Length < 4_194_303) {
						//make unique filename
						block.Image = Guid.NewGuid() + ext;

						//Save the file to the web server (here, saving to wwwroot/images)
						//To access wwwroot, add a property to the controller for the
						//_webHostEnvironment (see the top of this class for our example)
						//Retrieve the path to wwwroot
						string webRootPath = _webHostEnvironment.WebRootPath;

						//full image path must include images folder
						string fullImagePath = webRootPath + "/images/blocks/";

						//create a memorystream to read the image into the server memory
						using(var memoryStream = new MemoryStream()) {
							//transfer file from request into server memory
							await block.BlockImage.CopyToAsync(memoryStream);

							//add using statement for image class using system.drawing
							using(var img = Image.FromStream(memoryStream)) {
								//now, send the image to the ImageUtility for resizing and thumbnail creation
								//items needed for the ImageUtility.ResizeImage()
								// 1) (int) maximum image size
								// 2) (int) maximum thumbnail image size
								// 3) (string) full path where the file will be saved
								// 4) (Image) an image
								// 5) (string) filename
								int maxImgSize = 500;
								int maxThumbSize = 100;

								ImageUtility.ResizeImage(fullImagePath, block.Image, img, maxImgSize, maxThumbSize);

								//without handling resizing:
								//myFile.Save("save/path", "filename");
							}
						}

					}

				}
				else block.Image = "placeholder.png";
					
				#endregion

				_context.Add(block);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", block.CategoryId);
			ViewData["RelatedBlockId"] = new SelectList(_context.Blocks, "BlockId", "Name", block.RelatedBlockId);
			ViewData["SourceId"] = new SelectList(_context.Sources, "SourceId", "Name", block.SourceId);
			return View(block);
		}

		// GET: Blocks/Edit/5
		public async Task<IActionResult> Edit(int? id) {
			if(id == null) {
				return NotFound();
			}

			var block = await _context.Blocks.FindAsync(id);
			if(block == null) {
				return NotFound();
			}
			ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", block.CategoryId);
			ViewData["RelatedBlockId"] = new SelectList(_context.Blocks, "BlockId", "Name", block.RelatedBlockId);
			ViewData["SourceId"] = new SelectList(_context.Sources, "SourceId", "Name", block.SourceId);
			return View(block);
		}

		// POST: Blocks/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("BlockId,Name,Description,CategoryId,IsRenewable,IsStackable,StackSize,ProperTool,Luminous,LightLevel,Transparent,IsWaterloggable,Flammable,SourceId,BlockImage,Image,RelatedBlockId")] Block block) {
			if(id != block.BlockId) 
				return NotFound();

			if(ModelState.IsValid) {

				#region file upload

				//save old image name to delete if replacement comes
				string? oldImgName = block.Image;

				if(block.BlockImage != null) {

					//allowed extensions list
					string[] validExts = {
						".jpg",
						".jpeg",
						".png",
						".gif"
					};

					//check file type
					string ext = Path.GetExtension(block.BlockImage.FileName); //get the extension

					//check if file has valid extensions
					// and if size will work with .net app
					if(validExts.Contains(ext.ToLower()) && block.BlockImage.Length < 4_194_303) {
						//make unique filename
						block.Image = Guid.NewGuid() + ext;

						//Save the file to the web server (here, saving to wwwroot/images)
						//To access wwwroot, add a property to the controller for the
						//_webHostEnvironment (see the top of this class for our example)
						//Retrieve the path to wwwroot
						string webRootPath = _webHostEnvironment.WebRootPath;

						//full image path must include images folder
						string fullImagePath = webRootPath + "/images/blocks/";

						//delete old image
						if(oldImgName != fullImagePath + "placeholder.png")
							ImageUtility.Delete(fullImagePath, oldImgName);

						//create a memorystream to read the image into the server memory
						using(var memoryStream = new MemoryStream()) {
							//transfer file from request into server memory
							await block.BlockImage.CopyToAsync(memoryStream);

							//add using statement for image class using system.drawing
							using(var img = Image.FromStream(memoryStream)) {
								//now, send the image to the ImageUtility for resizing and thumbnail creation
								//items needed for the ImageUtility.ResizeImage()
								// 1) (int) maximum image size
								// 2) (int) maximum thumbnail image size
								// 3) (string) full path where the file will be saved
								// 4) (Image) an image
								// 5) (string) filename
								int maxImgSize = 500;
								int maxThumbSize = 100;

								ImageUtility.ResizeImage(fullImagePath, block.Image, img, maxImgSize, maxThumbSize);

							}
						}

					}

				}
				else
					block.Image = "placeholder.png";


				#endregion

				try {
					_context.Update(block);
					await _context.SaveChangesAsync();
				}
				catch(DbUpdateConcurrencyException) {
					if(!BlockExists(block.BlockId)) 
						return NotFound();
					else throw;
				}
				return RedirectToAction(nameof(Index));
			}
			ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", block.CategoryId);
			ViewData["RelatedBlockId"] = new SelectList(_context.Blocks, "BlockId", "Name", block.RelatedBlockId);
			ViewData["SourceId"] = new SelectList(_context.Sources, "SourceId", "Name", block.SourceId);
			return View(block);
		}

		// GET: Blocks/Delete/5
		public async Task<IActionResult> Delete(int? id) {
			if(id == null) {
				return NotFound();
			}

			var block = await _context.Blocks
				.Include(b => b.Category)
				.Include(b => b.RelatedBlock)
				.Include(b => b.Source)
				.FirstOrDefaultAsync(m => m.BlockId == id);
			if(block == null) {
				return NotFound();
			}

			return View(block);
		}

		// POST: Blocks/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id) {
			var block = await _context.Blocks.FindAsync(id);
			if(block != null) {
				_context.Blocks.Remove(block);
			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool BlockExists(int id) {
			return _context.Blocks.Any(e => e.BlockId == id);
		}
	}
}
