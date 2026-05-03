using System.ComponentModel.DataAnnotations;

namespace BlogApp.Models
{
    public class EditUserPasswordViewModel
    {
        public int UserId { get; set; }
        [Required]
        [StringLength(12, ErrorMessage = "{0} must be a minimum of {2} characters long.", MinimumLength = 6)]
        [Display(Name = "Current Password")]
        [DataType(DataType.Password)]
        //[RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+={}\[\]:;""'<>,.?\/\\|~`-]).+$", ErrorMessage = "The password must contain at least one number, one uppercase letter, and one symbol.")]
        public string? OldPassword { get; set; }
        [Required]
        [StringLength(12, ErrorMessage = "{0} must be a minimum of {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        //[RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+={}\[\]:;""'<>,.?\/\\|~`-]).+$", ErrorMessage = "The password must contain at least one number, one uppercase letter, and one symbol.")]
        public string? Password { get; set; }

        [Required]
        [StringLength(12, ErrorMessage = "{0} must be a minimum of {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Password doesn't match.")]
        //[RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+={}\[\]:;""'<>,.?\/\\|~`-]).+$", ErrorMessage = "The password must contain at least one number, one uppercase letter, and one symbol.")]
        public string? ConfirmPassword { get; set; }
    }
}
