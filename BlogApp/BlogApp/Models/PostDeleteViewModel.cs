using System.ComponentModel.DataAnnotations;

namespace BlogApp.Models
{
    public class PostDeleteViewModel
    {
        public int PostId { get; set; }
        public string? Title { get; set; }

    }
}
