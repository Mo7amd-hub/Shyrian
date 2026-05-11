using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Shyrian_project.Models;
using Microsoft.EntityFrameworkCore;

namespace Shyrian_project.Controllers
{
    public class AdminController : Controller
    {
        private readonly appDbContext _context;

        public AdminController(appDbContext context)
        {
            _context = context;
        }

        // 1. Admin Login (GET)
        [HttpGet]
        public IActionResult Login() => View();

        // 2. Admin Login (POST)
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            
            string hashedInput = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(password)));

            var admin = await _context.Admins
                .FirstOrDefaultAsync(a => a.Email == email && a.Password == hashedInput);

            if (admin != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, admin.Email),
                    new Claim(ClaimTypes.Role, "Admin")
                };

                var identity = new ClaimsIdentity(claims, "CookieAuth");
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("CookieAuth", principal);
                return RedirectToAction("Dashboard");
            }

            ViewBag.Error = "Invalid Admin Credentials";
            return View();
        }

        // 3. Dashboard (Stats)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Dashboard()
        {
            ViewBag.TotalUsers = await _context.Users.CountAsync();
            ViewBag.PendingVerifications = await _context.Users.CountAsync(u => u.Status == VerificationStatus.Pending);
            ViewBag.ActiveRequests = await _context.BloodRequests.CountAsync(r => r.Status == RequestStatus.Open);

            return View();
        }

        // 4. Verification Requests List
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> VerificationRequests()
        {
            var pendingUsers = await _context.Users
                .Where(u => u.Status == VerificationStatus.Pending)
                .Include(u => u.BloodType)
                .ToListAsync();
            return View(pendingUsers);
        }

        // 5. Approve/Reject Verification
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int userId, bool isApproved)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                if (isApproved)
                {
                    user.Status = VerificationStatus.Verified;
                    user.IsVerified = true;
                }
                else
                {
                    user.Status = VerificationStatus.Rejected;
                    user.IsVerified = false;
                }
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("VerificationRequests");
        }

        // 6. Manage All Requests
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageRequests()
        {
            var allRequests = await _context.BloodRequests
                .Include(r => r.Requester)
                .Include(r => r.BloodType)
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();
            return View(allRequests);
        }

        // 7. Delete Fake/Spam Requests
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> DeleteRequest(int requestId)
        {
            var request = await _context.BloodRequests.FindAsync(requestId);
            if (request != null)
            {
                _context.BloodRequests.Remove(request);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("ManageRequests");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            return RedirectToAction("Login");
        }
    }
}