using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Web_Frameworks_2025_EON.Data;
using Web_Frameworks_2025_EON.Models;
using Web_Frameworks_2025_EON.Models.ViewModels;
using Web_Frameworks_2025_EON.Repositories;
using System.Linq;
using System.Threading.Tasks;

namespace Web_Frameworks_2025_EON.Controllers
{
    [Authorize]
    public class ItemsController : Controller
    {
        private readonly IItemRepository _itemRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context; // For Requests

        public ItemsController(IItemRepository itemRepository, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _itemRepository = itemRepository;
            _userManager = userManager;
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(string category = null, string searchString = null)
        {
            var approvedItems = await _itemRepository.GetAllApprovedAsync();
            if (!string.IsNullOrEmpty(category)) { approvedItems = approvedItems.Where(i => i.Category != null && i.Category.Name == category); }
            if (!string.IsNullOrEmpty(searchString)) { approvedItems = approvedItems.Where(s => s.Name != null && s.Name.ToLower().Contains(searchString.ToLower())); }
            var itemsViewModel = approvedItems.Select(item => new ItemViewModel { Id = item.Id, Name = item.Name, Brand = item.Brand, Condition = item.Condition, CategoryName = item.Category?.Name, OwnerEmail = item.Owner?.Email }).ToList();
            return View(itemsViewModel);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var item = await _itemRepository.GetByIdAsync(id.Value);
            if (item == null) return NotFound();
            return View(item);
        }

        public async Task<IActionResult> Create()
        {
            ViewData["CategoryId"] = new SelectList(await _itemRepository.GetAllCategoriesAsync(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ListingType,Name,Description,Condition,Brand,CategoryId,Quantity")] Item item)
        {
            if (ModelState.IsValid)
            {
                item.OwnerId = _userManager.GetUserId(User);
                item.DatePosted = DateTime.UtcNow;
                item.IsApproved = User.IsInRole("Admin");
                await _itemRepository.AddAsync(item);
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(await _itemRepository.GetAllCategoriesAsync(), "Id", "Name", item.CategoryId);
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MakeRequest(int itemId)
        {
            var item = await _itemRepository.GetByIdAsync(itemId);
            var requesterId = _userManager.GetUserId(User);

            if (item == null || item.Quantity <= 0 || item.OwnerId == requesterId)
            {
                TempData["RequestResult"] = "Unable to make request at this time.";
                return RedirectToAction("Details", new { id = itemId });
            }

            var existingRequest = await _context.Requests
                .FirstOrDefaultAsync(r => r.ItemId == itemId && r.RequesterId == requesterId && r.Status == RequestStatus.Pending);

            if (existingRequest != null)
            {
                TempData["RequestResult"] = "You have already sent a request for this item.";
                return RedirectToAction("Details", new { id = itemId });
            }

            var request = new Request
            {
                ItemId = itemId,
                RequesterId = requesterId,
                Status = RequestStatus.Pending,
                Timestamp = DateTime.UtcNow
            };

            _context.Requests.Add(request);
            await _context.SaveChangesAsync();

            TempData["RequestResult"] = "Your request has been sent to the owner!";
            return RedirectToAction("Details", new { id = itemId });
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var item = await _itemRepository.GetByIdAsync(id.Value);
            if (item == null || item.OwnerId != _userManager.GetUserId(User)) return Forbid();
            ViewData["CategoryId"] = new SelectList(await _itemRepository.GetAllCategoriesAsync(), "Id", "Name", item.CategoryId);
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ListingType,Name,Description,Condition,Brand,CategoryId,Quantity")] Item item)
        {
            if (id != item.Id) return NotFound();

            var itemToUpdate = await _itemRepository.GetByIdAsync(id);
            if (itemToUpdate == null || itemToUpdate.OwnerId != _userManager.GetUserId(User)) return Forbid();

            if (ModelState.IsValid)
            {
                itemToUpdate.ListingType = item.ListingType;
                itemToUpdate.Name = item.Name;
                itemToUpdate.Description = item.Description;
                itemToUpdate.Condition = item.Condition;
                itemToUpdate.Brand = item.Brand;
                itemToUpdate.CategoryId = item.CategoryId;
                itemToUpdate.Quantity = item.Quantity; // <-- Add this line
                if (!User.IsInRole("Admin")) itemToUpdate.IsApproved = false;

                await _itemRepository.UpdateAsync(itemToUpdate);
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(await _itemRepository.GetAllCategoriesAsync(), "Id", "Name", item.CategoryId);
            return View(item);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var item = await _itemRepository.GetByIdAsync(id.Value);
            if (item == null || (item.OwnerId != _userManager.GetUserId(User) && !User.IsInRole("Admin"))) return Forbid();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _itemRepository.GetByIdAsync(id);
            if (item == null || (item.OwnerId != _userManager.GetUserId(User) && !User.IsInRole("Admin"))) return Forbid();

            await _itemRepository.DeleteAsync(item);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Pending()
        {
            var pendingItems = await _itemRepository.GetAllPendingAsync();
            return View(pendingItems);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(int id)
        {
            var item = await _itemRepository.GetByIdAsync(id);
            if (item != null)
            {
                item.IsApproved = true;
                await _itemRepository.UpdateAsync(item);
            }
            return RedirectToAction(nameof(Pending));
        }



    }

}