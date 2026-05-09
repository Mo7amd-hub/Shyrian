namespace Shyrian_project.Models
{
    public class BloodType
    {
        public int Id { get; set; }
        public string Name { get; set; } 

       
        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<BloodRequest> BloodRequests { get; set; }
    }
}
