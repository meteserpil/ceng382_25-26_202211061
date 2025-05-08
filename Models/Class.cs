using System.ComponentModel.DataAnnotations;

namespace MyRazorApp.Models
{
    public class Class
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [Range(1, 1000)]
        public int PersonCount { get; set; }
        
        public string? Description { get; set; }
        
        [Required]
        public bool IsActive { get; set; } = true;
    }
}