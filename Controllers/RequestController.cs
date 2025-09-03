using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_Frameworks_2025_EON.Data;
using Web_Frameworks_2025_EON.Models;

namespace Web_Frameworks_2025_EON.Controllers
{
    [Authorize]
    public class RequestController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public RequestController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUserId = _userManager.GetUserId(User);

            var receivedRequests = await _context.Requests
                .Where(r => r.Item.OwnerId == currentUserId)
                .Include(r => r.Item)
                .Include(r => r.Requester)
                .OrderByDescending(r => r.Timestamp)
                .ToListAsync();

            return View(receivedRequests);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Accept(int requestId)
        {
            var request = await _context.Requests.Include(r => r.Item).FirstOrDefaultAsync(r => r.Id == requestId);
            var currentUserId = _userManager.GetUserId(User);

            if (request != null && request.Item.OwnerId == currentUserId)
            {
                if (request.Item.Quantity > 0)
                {
                    request.Status = RequestStatus.Accepted;
                    request.Item.Quantity--; 
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deny(int requestId)
        {
            var request = await _context.Requests.Include(r => r.Item).FirstOrDefaultAsync(r => r.Id == requestId);
            var currentUserId = _userManager.GetUserId(User);

            if (request != null && request.Item.OwnerId == currentUserId)
            {
                request.Status = RequestStatus.Denied;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    } 
}