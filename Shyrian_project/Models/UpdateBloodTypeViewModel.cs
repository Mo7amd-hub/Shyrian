using System.ComponentModel.DataAnnotations;

public class UpdateBloodTypeViewModel
{
    [Required(ErrorMessage = "Please select your blood type.")]
    [Display(Name = "Blood Type")]
    public int BloodTypeId { get; set; }

    [Display(Name = "Verification Document")]
    public IFormFile? DocumentFile { get; set; }
}