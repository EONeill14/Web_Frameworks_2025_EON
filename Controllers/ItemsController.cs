using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Web_Frameworks_2025_EON.Data;
using Web_Frameworks_2025_EON.Models;
using Web_Frameworks_2025_EON.Models.ViewModels;

namespace Web_Frameworks_2025_EON.Controllers
{
    [Authorize] // Require login for all actions in this controller by default
    public class ItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ItemsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Items - Shows all items to any user
        [AllowAnonymous] // Overrides the [Authorize] at the top to allow guests
        public async Task<IActionResult> Index()
        {
            var items = await _context.Items
                .Include(i => i.Category)
                .Include(i => i.Owner)
                .Select(item => new ItemViewModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    Brand = item.Brand,
                    Condition = item.Condition,
                    CategoryName = item.Category.Name,
                    OwnerEmail = item.Owner.Email
                }).ToListAsync();

            return View(items);
        }

        // GET: Items/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Items
                .Include(i => i.Category)
                .Include(i => i.Owner)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // GET: Items/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Items/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Condition,Brand,CategoryId")] Item item)
        {
            if (ModelState.IsValid)
            {
                // Set the owner to the current logged-in user
                item.OwnerId = _userManager.GetUserId(User);
                item.DatePosted = DateTime.UtcNow;

                _context.Add(item);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", item.CategoryId);
            return View(item);
        }

        // GET: Items/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            // Use FirstOrDefaultAsync with Include to load related data
            var item = await _context.Items
                .Include(i => i.Category)
                .Include(i => i.Owner)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (item == null) return NotFound();

            // SECURITY CHECK: Ensure the current user is the owner
            if (item.OwnerId != _userManager.GetUserId(User))
            {
                return Forbid(); // User is not authorized
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", item.CategoryId);
            return View(item);
        }

        // POST: Items/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Condition,Brand,CategoryId")] Item item)
        {
            if (id != item.Id)
            {
                return NotFound();
            }

            // Fetch the original item from the database
            var itemToUpdate = await _context.Items.FindAsync(id);

            if (itemToUpdate == null)
            {
                return NotFound();
            }

            // SECURITY CHECK: Ensure the current user is the owner
            if (itemToUpdate.OwnerId != _userManager.GetUserId(User))
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Update only the properties that can be changed
                    itemToUpdate.Name = item.Name;
                    itemToUpdate.Description = item.Description;
                    itemToUpdate.Condition = item.Condition;
                    itemToUpdate.Brand = item.Brand;
                    itemToUpdate.CategoryId = item.CategoryId;

                    _context.Update(itemToUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemExists(item.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", item.CategoryId);
            return View(item);
        }

        // GET: Items/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var item = await _context.Items
                .Include(i => i.Category)
                .Include(i => i.Owner)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (item == null) return NotFound();

            // SECURITY CHECK: Ensure the current user is the owner
            if (item.OwnerId != _userManager.GetUserId(User))
            {
                return Forbid();
            }

            return View(item);
        }

        // POST: Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null) return NotFound();

            // SECURITY CHECK: Ensure the current user is the owner before deleting
            if (item.OwnerId != _userManager.GetUserId(User))
            {
                return Forbid();
            }

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ItemExists(int id)
        {
            return _context.Items.Any(e => e.Id == id);
        }

        [Authorize(Roles = "Admin")] // This ensures only Admins can access this page
        public async Task<IActionResult> Pending()
        {
            // Find all items where IsApproved is false
            var pendingItems = await _context.Items
                .Where(i => !i.IsApproved)
                .Include(i => i.Category)
                .Include(i => i.Owner)
                .ToListAsync();

            return View(pendingItems);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item != null)
            {
                item.IsApproved = true;
                _context.Update(item);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Pending));
        }

    }
}