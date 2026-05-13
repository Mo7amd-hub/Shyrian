using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shyrian_project.Models;
using System.Security.Claims;

namespace Shyrian_project.Controllers
{
    // طالما حطيناها هنا، مفيش داعي نكررها فوق كل دالة
    [Authorize]
    public class BloodRequestController : Controller
    {
        private readonly appDbContext _context;

        public BloodRequestController(appDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? governorateId, int? bloodTypeId)
        {
            var requestsQuery = _context.BloodRequests
                .Include(r => r.Requester)
                .Include(r => r.BloodType)
                .Include(r => r.HospitalGovernorate)
                .Include(r => r.HospitalCity)
                .Where(r => r.Status == RequestStatus.Open)
                .AsQueryable();

            if (governorateId.HasValue && governorateId > 0)
            {
                requestsQuery = requestsQuery.Where(r => r.HospitalGovernorateId == governorateId);
            }

            if (bloodTypeId.HasValue && bloodTypeId > 0)
            {
                requestsQuery = requestsQuery.Where(r => r.BloodTypeId == bloodTypeId);
            }

            var requests = await requestsQuery
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();

            ViewBag.Governorates = new SelectList(await _context.Governorates.ToListAsync(), "Id", "Name", governorateId);
            ViewBag.BloodTypes = new SelectList(await _context.BloodTypes.ToListAsync(), "Id", "Name", bloodTypeId);

            return View(requests);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Governorates = await _context.Governorates.ToListAsync();
            ViewBag.BloodTypes = await _context.BloodTypes.ToListAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BloodRequestViewModel model)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return RedirectToAction("Login", "Account");
            }
            /*
            ModelState.Remove("Requester");
            ModelState.Remove("BloodType");
            ModelState.Remove("HospitalGovernorate");
            ModelState.Remove("HospitalCity");
            ModelState.Remove("SelectedDonor");
            ModelState.Remove("DonationOffers");
            */
            // السيستم هيتجاهل الكائنات هنا بسبب الـ [ValidateNever] اللي حطيناها في الموديل
            if (ModelState.IsValid)
            {
                BloodRequest bloodrequest = new BloodRequest
                {
                    RequesterId = userId,
                    RequestDate = DateTime.Now,
                    Status = RequestStatus.Open,
                    PatientName = model.PatientName,
                    ContactNumber = model.ContactNumber,
                    HospitalAddress = model.HospitalAddress,
                    HospitalCityId = model.HospitalCityId,
                    HospitalGovernorateId = model.HospitalGovernorateId,
                    HospitalName = model.HospitalName,
                    BloodTypeId = model.BloodTypeId
                };
                _context.BloodRequests.Add(bloodrequest);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Your blood request has been posted successfully.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Governorates = await _context.Governorates.ToListAsync();
            ViewBag.BloodTypes = await _context.BloodTypes.ToListAsync();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var bloodRequest = await _context.BloodRequests
                .Include(r => r.BloodType)
                .Include(r => r.HospitalGovernorate)
                .Include(r => r.HospitalCity)
                .Include(r => r.Requester)
                .Include(r => r.DonationOffers)
                    .ThenInclude(o => o.Donor)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (bloodRequest == null)
            {
                return NotFound();
            }

            ViewBag.IsRequester = (bloodRequest.RequesterId == userId);
            ViewBag.AlreadyOffered = bloodRequest.DonationOffers.Any(o => o.DonorId == userId);

            return View(bloodRequest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClickDonate(int requestId)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            if (user.BloodTypeId == null)
            {
                TempData["ErrorMessage"] = "You must register your blood type before donating.";
                return RedirectToAction("UpdateBloodType", "Account");
            }

            if (user.LastDonationDate.HasValue)
            {
                var threeMonthsAgo = DateTime.Now.AddMonths(-3);

                if (user.LastDonationDate.Value > threeMonthsAgo)
                {
                    TempData["ErrorMessage"] = "For your safety, 3 months must pass since your last donation before you can donate again.";
                    return RedirectToAction("Details", new { id = requestId });
                }
            }

            var request = await _context.BloodRequests
                .Include(r => r.DonationOffers)
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null || request.Status != RequestStatus.Open)
            {
                TempData["ErrorMessage"] = "This request is closed or no longer available.";
                return RedirectToAction("Index");
            }

            if (request.RequesterId == userId)
            {
                TempData["ErrorMessage"] = "You cannot donate to your own request.";
                return RedirectToAction("Details", new { id = requestId });
            }

            if (request.DonationOffers.Any(o => o.DonorId == userId))
            {
                TempData["ErrorMessage"] = "You have already offered to donate for this request.";
                return RedirectToAction("Details", new { id = requestId });
            }

            var newOffer = new DonationOffer
            {
                DonorId = userId,
                BloodRequestId = requestId,
                OfferDate = DateTime.Now
            };

            _context.DonationOffers.Add(newOffer);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Thank you! Your intent to donate has been registered successfully.";
            return RedirectToAction("Details", new { id = requestId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CloseRequest(int requestId, int selectedDonorId)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var request = await _context.BloodRequests
                .Include(r => r.DonationOffers)
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null) return NotFound();

            if (request.RequesterId != userId)
            {
                TempData["ErrorMessage"] = "You do not have permission to close this request.";
                return RedirectToAction("Index");
            }

            if (request.Status != RequestStatus.Open)
            {
                TempData["ErrorMessage"] = "This request is already closed or fulfilled.";
                return RedirectToAction("Details", new { id = requestId });
            }

            bool isActualDonor = request.DonationOffers.Any(o => o.DonorId == selectedDonorId);
            if (!isActualDonor)
            {
                TempData["ErrorMessage"] = "The selected user did not offer to donate for this request.";
                return RedirectToAction("Details", new { id = requestId });
            }

            var donor = await _context.Users.FindAsync(selectedDonorId);
            if (donor == null) return NotFound();

            request.Status = RequestStatus.Fulfilled;
            request.SelectedDonorId = selectedDonorId;
            donor.LastDonationDate = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Request fulfilled successfully! Thank you and the donor for saving a life.";
            return RedirectToAction("Details", new { id = requestId });
        }
    }
}