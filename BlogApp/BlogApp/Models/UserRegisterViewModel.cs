using BlogApp.Entity;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace BlogApp.Models
{
    public class UserRegisterViewModel
    {

        [Required]
        [StringLength(100)]
        [RegularExpression(@"^[a-zA-Z_][a-zA-Z0-9_]*$", ErrorMessage = "The username must contain only letters and numbers.")]
        public string? UserName { get; set; }


        [Required]
        [StringLength(100)]
        public string? Name { get; set; }


        [Required]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(100)]
        public string? Email { get; set; }


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

        public string? Image { get; set; }
    }
}
