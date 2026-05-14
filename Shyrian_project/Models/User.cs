using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shyrian_project.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }


        [Required(ErrorMessage = "Enter your full name")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }


        [Required(ErrorMessage = "Enter your Email")]
        [EmailAddress(ErrorMessage = "Enter a valid Email")]
        [Display(Name = "Email")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Password is Required")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters")]
        [Display(Name = "password")]
        public string Password { get; set; }


        [Required(ErrorMessage = "Phone number is Required")]
        [RegularExpression(@"^01[0125][0-9]{8}$", ErrorMessage = "Enter a valid Number")]
        [Display(Name = "phone number")]
        public string PhoneNumber { get; set; }



        [ForeignKey("BloodType")]
        [Display(Name = "blood Type")]
        public int? BloodTypeId { get; set; }
        public virtual BloodType BloodType { get; set; }



        [Required(ErrorMessage = "Enter your city")]
        [ForeignKey("City")]
        [Display(Name = "City")]
        public int CityId { get; set; }
        public virtual City City { get; set; }



        [Required(ErrorMessage = "Enter your Governorate")]
        [ForeignKey("Governorate")]
        [Display(Name = "Governorate")]
        public int GovernorateId { get; set; }
        public virtual Governorate Governorate { get; set; }

        
        public string? DocumentPath { get; set; } // path of the uploaded document

        public VerificationStatus Status { get; set; } = VerificationStatus.NotSubmitted; // Default value when user registers

        public bool IsVerified { get; set; } = false;

        [Display(Name = "date of last donation")]
        public DateTime? LastDonationDate { get; set; }

        // Navigation Properties
       
        public virtual ICollection<BloodRequest> MyRequests { get; set; }

        // 2. Relation between user and his Donation offers
        public virtual ICollection<DonationOffer> DonationOffers { get; set; }
    }

    
    public enum VerificationStatus
    {
        NotSubmitted,
        Pending,
        Verified,
        Rejected
    }
}