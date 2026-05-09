using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shyrian_project.Models
{
    public class User
    {
        
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }

        // فصيلة الدم (Nullable) عشان تكون اختياري وقت التسجيل
        [ForeignKey("BloodType")]
        public int? BloodTypeId { get; set; }
        public virtual BloodType BloodType { get; set; }

        // مكان السكن
        [ForeignKey("City")]
        public int CityId { get; set; }
        public virtual City City { get; set; }

        [ForeignKey("Governorate")]
        public int GovernorateId { get; set; }
        public virtual Governorate Governorate { get; set; }

        // نظام التوثيق بالوثائق
        public string DocumentPath { get; set; } // مسار الصورة المرفوعة
        public VerificationStatus Status { get; set; } // حالة التوثيق (موثق، مرفوض، قيد المراجعة)

        public DateTime? LastDonationDate { get; set; } // اختياري برضه

        public virtual ICollection<BloodRequest> MyRequests { get; set; }
    }

    // Enum عشان ننظم حالات التوثيق
    public enum VerificationStatus
    {
        NotSubmitted, // لم يرفع وثيقة
        Pending,      // قيد المراجعة من الأدمن
        Verified,     // تم التوثيق بنجاح
        Rejected      // الوثيقة غير صحيحة
    }
}
