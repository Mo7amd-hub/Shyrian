using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shyrian_project.Models
{
    public class BloodRequestViewModel
    {
        [Required(ErrorMessage = "Enter patient name")]
        [Display(Name = "patient name")]
        public string PatientName { get; set; }


        [Required(ErrorMessage = "Enter the Required blood type")]
        [Display(Name = "blodd type")]
        public int BloodTypeId { get; set; }



        [Required(ErrorMessage = "choose the Hospital Governorate")]
        [Display(Name = "Hospital Governorate")]
        public int HospitalGovernorateId { get; set; }



        [Required(ErrorMessage = "choose the Hospital City")]
        [Display(Name = "Hospital City")]
        public int HospitalCityId { get; set; }


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


    }
}
