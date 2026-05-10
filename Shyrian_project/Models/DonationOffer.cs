using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shyrian_project.Models
{
    public class DonationOffer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("BloodRequest")]
        public int BloodRequestId { get; set; }

        public virtual BloodRequest BloodRequest { get; set; }


        [Required]
        [ForeignKey("Donor")]
        public int DonorId { get; set; }
        public virtual User Donor { get; set; }


        [Display(Name = "donation offer date")]
        public DateTime OfferDate { get; set; } = DateTime.Now;
    }
}