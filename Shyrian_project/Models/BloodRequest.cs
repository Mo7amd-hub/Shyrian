using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shyrian_project.Models
{
    public class BloodRequest
    {
        [Key]
        public int Id { get; set; }

        
        [Required]
        [ForeignKey("Requester")]
        public int RequesterId { get; set; }
        [InverseProperty("MyRequests")] // عشان نميزها عن علاقة المتبرع في الداتابيز
        public virtual User Requester { get; set; }



        [Required(ErrorMessage = "Enter patient name")]
        [Display(Name = "patient name")]
        public string PatientName { get; set; }


        [Required(ErrorMessage = "Enter the Required blood type")]
        [ForeignKey("BloodType")]
        [Display(Name = "blodd type")]
        public int BloodTypeId { get; set; }
        public virtual BloodType BloodType { get; set; }



        [Required(ErrorMessage = "choose the Hospital Governorate")]
        [ForeignKey("HospitalGovernorate")]
        [Display(Name = "Hospital Governorate")]
        public int HospitalGovernorateId { get; set; }
        public virtual Governorate HospitalGovernorate { get; set; }



        [Required(ErrorMessage = "choose the Hospital City")]
        [ForeignKey("HospitalCity")]
        [Display(Name = "Hospital City")]
        public int HospitalCityId { get; set; }
        public virtual City HospitalCity { get; set; }


        
        [Required(ErrorMessage = "Enter the Hospital Name")]
        [StringLength(50)]
        [Display(Name = "Hospital Name")]
        public string HospitalName { get; set; }



        [Display(Name = "the detailed hospital address")]
        public string HospitalAddress { get; set; }



        [Required(ErrorMessage = "Enter contact number")]
        [RegularExpression(@"^01[0125][0-9]{8}$", ErrorMessage = "Enter a valid number")]
        [Display(Name = "contact number")]
        public string ContactNumber { get; set; }



        [Display(Name = "Request date")]
        public DateTime RequestDate { get; set; } = DateTime.Now;



        [Display(Name = "Request status")]
        public RequestStatus Status { get; set; } = RequestStatus.Open;



        // the selected donor for this request (the one who accepted to donate)
        [ForeignKey("SelectedDonor")]
        public int? SelectedDonorId { get; set; }
        public virtual User SelectedDonor { get; set; }

        public virtual ICollection<DonationOffer> DonationOffers { get; set; }
    }

    
    public enum RequestStatus
    {
        Open,
        Closed,
        Fulfilled
    }
}