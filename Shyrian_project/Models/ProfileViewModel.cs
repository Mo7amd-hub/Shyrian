using Shyrian_project.Models;

public class ProfileViewModel
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string BloodTypeName { get; set; }
    public string Location { get; set; } // عشان نعرض المحافظة والمدينة مع بعض
    public VerificationStatus Status { get; set; }
    public DateTime? LastDonationDate { get; set; }

    // هنجيب الريكويستات بتاعته هنا
    public IEnumerable<BloodRequest> RequestHistory { get; set; }
}