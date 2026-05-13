using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Shyrian_project.Models;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
namespace Shyrian_project.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        private readonly appDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AccountController(appDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public IActionResult Register()
        {
            ViewBag.Governorates = _context.Governorates.ToList();
            ViewBag.BloodTypes = _context.BloodTypes.ToList();

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 1. التشيك لو الإيميل موجود قبل كدة
                var emailExists = _context.Users.Any(u => u.Email == model.Email);
                if (emailExists)
                {
                    ModelState.AddModelError("Email", "This email is already registered.");
                    ViewBag.Governorates = _context.Governorates.ToList();
                    return View(model);
                }

                var newUser = new User
                {
                    FullName = $"{model.FirstName} {model.LastName}",
                    Email = model.Email,
                    Password = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(model.Password))),
                    PhoneNumber = model.PhoneNumber,
                    BloodTypeId = model.BloodTypeId,
                    GovernorateId = model.GovernorateId,
                    CityId = model.CityId,
                    Status = VerificationStatus.NotSubmitted 
                };

                // 2. التعامل مع رفع الصورة
                if (model.DocumentFile != null && model.DocumentFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "documents");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.DocumentFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.DocumentFile.CopyToAsync(fileStream);
                    }

                    newUser.DocumentPath = uniqueFileName;
                    newUser.Status = VerificationStatus.Pending; 
                }

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                return RedirectToAction("Login");
            }
            ViewBag.Governorates = _context.Governorates.ToList();
            ViewBag.BloodTypes = _context.BloodTypes.ToList();
            return View(model);
        }
        public JsonResult GetCities(int governorateId)
        {
            var cities = _context.Cities
                .Where(c => c.GovernorateId == governorateId)
                .Select(c => new { id = c.Id, name = c.Name })
                .ToList();
            return Json(cities);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var hashedPassword = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(model.Password)));
                var user = _context.Users.FirstOrDefault(u => u.Email == model.Email && u.Password == hashedPassword);

                if (user != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.FullName),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim("Status", user.Status.ToString())
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Invalid email or password.");
            }

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _context.Users
                .Include(u => u.BloodType)
                .Include(u => u.Governorate)
                .Include(u => u.City)
                .Include(u => u.MyRequests)
                .Include(u => u.DonationOffers) // التبرعات اللي قدمها لغيره
                    .ThenInclude(o => o.BloodRequest) // عشان نجيب بيانات الطلب اللي اتبرع فيه
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound();
            }

            var profileVM = new ProfileViewModel
            {
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                BloodTypeName = user.BloodType != null ? user.BloodType.Name : "Not Specified",
                BloodTypeId = user.BloodTypeId, // ضيف السطر ده هنا
                Location = $"{user.Governorate?.Name} - {user.City?.Name}",
                Status = user.Status,
                LastDonationDate = user.LastDonationDate,
                RequestHistory = user.MyRequests.OrderByDescending(r => r.Id).ToList(),
                DonationHistory = user.DonationOffers
                .Where(o => o.BloodRequest.SelectedDonorId == userId && o.BloodRequest.Status == RequestStatus.Fulfilled)
                .OrderByDescending(o => o.OfferDate).ToList() // تعبئة التبرعات
            };

            return View(profileVM);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> UpdateBloodType()
        {
            // 1. نجيب الـ ID بتاع اليوزر الحالي
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId)) return RedirectToAction("Login");

            // 2. نجيب بيانات اليوزر من الداتابيز
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            // 3. نملا الـ ViewModel بالبيانات الحالية (عشان الـ View يعرف إن فيه فصيلة)
            var model = new UpdateBloodTypeViewModel
            {
                BloodTypeId = user.BloodTypeId ?? 0 // لو null هتبقى 0، لو موجودة هتاخد الـ ID بتاعها
            };

            ViewBag.BloodTypes = await _context.BloodTypes.ToListAsync();

            return View(model); // مهم جداً نبعت الـ model هنا
        }


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateBloodType(UpdateBloodTypeViewModel model)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            // 1. منع تغيير الفصيلة لو كانت مسجلة مسبقاً
            if (user.BloodTypeId != null && user.BloodTypeId != model.BloodTypeId)
            {
                TempData["ErrorMessage"] = "Security Alert: You cannot change your blood type once it is set.";
                return RedirectToAction("Profile");
            }

            // 2. التحقق من صحة البيانات (Validation)
            if (!ModelState.IsValid)
            {
                ViewBag.BloodTypes = await _context.BloodTypes.ToListAsync();
                return View(model);
            }

            // 3. تحديث الفصيلة لأول مرة فقط
            bool isBloodTypeFirstTimeSet = user.BloodTypeId == null && model.BloodTypeId != 0;
            if (isBloodTypeFirstTimeSet)
            {
                user.BloodTypeId = model.BloodTypeId;
                user.Status = VerificationStatus.NotSubmitted;
            }

            // 4. معالجة رفع الملف (المستند الطبي)
            if (model.DocumentFile != null && model.DocumentFile.Length > 0)
            {
                // حذف الملف القديم إن وجد لتوفير المساحة
                if (!string.IsNullOrEmpty(user.DocumentPath))
                {
                    var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "documents", user.DocumentPath);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                // إعداد المسار وحفظ الملف الجديد
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "documents");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.DocumentFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.DocumentFile.CopyToAsync(fileStream);
                }

                user.DocumentPath = uniqueFileName;
                user.Status = VerificationStatus.Pending; // الحالة تصبح معلقة للمراجعة
            }

            // 5. حفظ التغييرات في قاعدة البيانات
            await _context.SaveChangesAsync();

            // 6. التوجيه النهائي للبروفايل مع رسالة النجاح
            TempData["SuccessMessage"] = "Changes saved successfully! Your profile is now pending admin verification.";
            return RedirectToAction("Profile");
        }
    }
}
