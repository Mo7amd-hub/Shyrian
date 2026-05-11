using System.ComponentModel.DataAnnotations;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Enter your first name")]
    [Display(Name = "First Name")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Enter your last name")]
    [Display(Name = "Last Name")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Enter your Email")]
    [EmailAddress(ErrorMessage = "Enter a valid Email")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is Required")]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Phone number is Required")]
    [RegularExpression(@"^01[0125][0-9]{8}$", ErrorMessage = "Enter a valid Number")]
    public string PhoneNumber { get; set; }

    public int? BloodTypeId { get; set; }

    [Required(ErrorMessage = "Please select a Governorate")]
    public int GovernorateId { get; set; }

    [Required(ErrorMessage = "Please select a City")]
    public int CityId { get; set; }

    public IFormFile? DocumentFile { get; set; }
}