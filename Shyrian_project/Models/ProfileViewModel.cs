using Shyrian_project.Models;
namespace Shyrian_project.Models;

public class ProfileViewModel
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string BloodTypeName { get; set; }
    public string Location { get; set; } // عشان نعرض المحافظة والمدينة مع بعض
    public int? BloodTypeId { get; set; } // ضيف ده
    public bool IsVerified { get; set; } // حالة التوثيق

    public VerificationStatus Status { get; set; }
    public DateTime? LastDonationDate { get; set; } 

    public IEnumerable<DonationOffer> DonationHistory { get; set; } // القائمة الجديدة


    // هنجيب الريكويستات بتاعته هنا
    public IEnumerable<BloodRequest> RequestHistory { get; set; }
}