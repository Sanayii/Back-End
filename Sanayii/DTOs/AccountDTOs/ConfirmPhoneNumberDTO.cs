using System.ComponentModel.DataAnnotations;

namespace Sanayii.DTOs.AccountDTO
{
    public class ConfirmPhoneNumberDTO
    {
        [Phone(ErrorMessage = "Please Enter a Valid Phone Number")]
        [Required(ErrorMessage = "Please Enter Phone Number")]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }
    }
}
