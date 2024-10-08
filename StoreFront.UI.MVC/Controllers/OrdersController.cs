﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StoreFront.Data.EF.Models;
using Microsoft.AspNetCore.Authorization;

namespace StoreFront.UI.MVC.Controllers;

public class OrdersController : Controller {
	private readonly McstoreContext _context;

	public OrdersController(McstoreContext context) {
		_context = context;
	}

	// GET: Orders
	[Authorize]
	public async Task<IActionResult> Index() {
		var mcstoreContext = _context.Orders.Include(o => o.Buyer);
		return View(await mcstoreContext.ToListAsync());
	}

	// GET: Orders/Details/5
	[Authorize]
	public async Task<IActionResult> Details(int? id) {
		if(id == null) {
			return NotFound();
		}

		var order = await _context.Orders
			.Include(o => o.Buyer)
			.FirstOrDefaultAsync(m => m.OrderId == id);
		if(order == null) {
			return NotFound();
		}

		return View(order);
	}

	// GET: Orders/Create
	[Authorize(Roles = "admin")]
	public IActionResult Create() {
		ViewData["BuyerId"] = new SelectList(_context.Users, "UserId", "UserId");
		return View();
	}

	// POST: Orders/Create
	// To protect from overposting attacks, enable the specific properties you want to bind to.
	// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
	[Authorize(Roles = "admin")]
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Create([Bind("OrderId,BuyerId,DateOrdered,DeliveryCoords")] Order order) {
		if(ModelState.IsValid) {
			_context.Add(order);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}
		ViewData["BuyerId"] = new SelectList(_context.Users, "UserId", "UserId", order.BuyerId);
		return View(order);
	}

	// GET: Orders/Edit/5
	[Authorize(Roles = "admin")]
	public async Task<IActionResult> Edit(int? id) {
		if(id == null) {
			return NotFound();
		}

		var order = await _context.Orders.FindAsync(id);
		if(order == null) {
			return NotFound();
		}
		ViewData["BuyerId"] = new SelectList(_context.Users, "UserId", "UserId", order.BuyerId);
		return View(order);
	}

	// POST: Orders/Edit/5
	// To protect from overposting attacks, enable the specific properties you want to bind to.
	// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
	[Authorize(Roles = "admin")]
	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Edit(int id, [Bind("OrderId,BuyerId,DateOrdered,DeliveryCoords")] Order order) {
		if(id != order.OrderId) {
			return NotFound();
		}

		if(ModelState.IsValid) {
			try {
				_context.Update(order);
				await _context.SaveChangesAsync();
			}
			catch(DbUpdateConcurrencyException) {
				if(!OrderExists(order.OrderId)) {
					return NotFound();
				}
				else {
					throw;
				}
			}
			return RedirectToAction(nameof(Index));
		}
		ViewData["BuyerId"] = new SelectList(_context.Users, "UserId", "UserId", order.BuyerId);
		return View(order);
	}

	// GET: Orders/Delete/5
	[Authorize(Roles = "admin")]
	public async Task<IActionResult> Delete(int? id) {
		if(id == null) {
			return NotFound();
		}

		var order = await _context.Orders
			.Include(o => o.Buyer)
			.FirstOrDefaultAsync(m => m.OrderId == id);
		if(order == null) {
			return NotFound();
		}

		return View(order);
	}

	// POST: Orders/Delete/5
	[Authorize(Roles = "admin")]
	[HttpPost, ActionName("Delete")]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> DeleteConfirmed(int id) {
		var order = await _context.Orders.FindAsync(id);
		if(order != null) {
			_context.Orders.Remove(order);
		}

		await _context.SaveChangesAsync();
		return RedirectToAction(nameof(Index));
	}

	private bool OrderExists(int id) {
		return _context.Orders.Any(e => e.OrderId == id);
	}
}

