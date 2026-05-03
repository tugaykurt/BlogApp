using System.ComponentModel.DataAnnotations;

namespace BlogApp.Models
{
    public class EditUserProfileViewModel
    {
        public int UserId { get; set; }
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

        [DataType(DataType.Password)]
        public string? Password { get; set; }
        public string? Image { get; set; }
    }
}
