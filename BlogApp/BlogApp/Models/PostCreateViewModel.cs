using BlogApp.Entity;
using System.ComponentModel.DataAnnotations;

namespace BlogApp.Models
{
    public class PostCreateViewModel
    {
        public int PostId { get; set; }
        [Required]
        [StringLength(200)]
        public string? Title { get; set; }


        [Required]
        [StringLength(500)]
        public string? Description { get; set; }


        [Required]
        public string? Content { get; set; }


        [Required]
        [StringLength(50)]
        [RegularExpression(@"^[a-z_][a-z0-9_-]*$", ErrorMessage = "URLs should only contain lowercase letters, numbers, and the semicolon (_) and hyphens (-).")]
        public string? Url { get; set; }

        public string? Image { get; set; }

        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public string? TagText { get; set; }
        public List<Tag> Tags { get; set; } = new();
    }
}
