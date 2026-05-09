using System.ComponentModel.DataAnnotations.Schema;

namespace Shyrian_project.Models
{
    public class BloodRequest
    {
        public int Id { get; set; }

        [ForeignKey("User")]
        public int RequesterId { get; set; }
        public virtual User Requester { get; set; }

        public string PatientName { get; set; }

        [ForeignKey("BloodType")]
        public int BloodTypeId { get; set; }
        public virtual BloodType BloodType { get; set; }

        [ForeignKey("Governorate")]
        public int HospitalGovernorateId { get; set; }
        public virtual Governorate HospitalGovernorate { get; set; }

        [ForeignKey("City")]
        public int HospitalCityId { get; set; }
        public virtual City HospitalCity { get; set; }
        public string HospitalAddress { get; set; }

        public int BagsCount { get; set; }
        public string ContactNumber { get; set; }
        public DateTime RequestDate { get; set; } = DateTime.Now;
        public bool IsClosed { get; set; } = false;
    }
}
