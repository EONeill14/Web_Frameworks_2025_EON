using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_Frameworks_2025_EON.Data;
using Web_Frameworks_2025_EON.Models;
using Web_Frameworks_2025_EON.Models.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace Web_Frameworks_2025_EON.Controllers
{
    [Authorize]
    public class MessageController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MessageController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUserId = _userManager.GetUserId(User);

            var messages = await _context.Messages
                .Where(m => m.SenderId == currentUserId || m.ReceiverId == currentUserId)
                .OrderByDescending(m => m.Timestamp)
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .ToListAsync();

            var conversations = messages
                .GroupBy(m => m.SenderId == currentUserId ? m.ReceiverId : m.SenderId)
                .Select(g => new ConversationViewModel
                {
                    OtherUserId = g.Key,
                    OtherUserEmail = g.First().SenderId == currentUserId ? g.First().Receiver.Email : g.First().Sender.Email,
                    LastMessage = g.First() 
                })
                .ToList();

            return View(conversations);
        }

        public async Task<IActionResult> Chat(string otherUserId)
        {
            var currentUserId = _userManager.GetUserId(User);
            var otherUser = await _userManager.FindByIdAsync(otherUserId);

            if (otherUser == null)
            {
                return NotFound();
            }

            var messages = await _context.Messages
                .Where(m => (m.SenderId == currentUserId && m.ReceiverId == otherUserId) ||
                             (m.SenderId == otherUserId && m.ReceiverId == currentUserId))
                .OrderBy(m => m.Timestamp)
                .ToListAsync();

            var viewModel = new ChatViewModel
            {
                Messages = messages,
                ReceiverId = otherUserId,
                ReceiverEmail = otherUser.Email
            };

            return View(viewModel);
        }
    }
}