using System.ComponentModel.DataAnnotations.Schema;

namespace Shyrian_project.Models
{
    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [ForeignKey("Governorate")]
        public int GovernorateId { get; set; }
        public virtual Governorate Governorate { get; set; }

        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<BloodRequest> BloodRequests { get; set; }
    }
}
