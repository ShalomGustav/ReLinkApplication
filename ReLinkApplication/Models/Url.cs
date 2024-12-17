using System.ComponentModel.DataAnnotations;

namespace ReLinkApplication.Models
{
    public class Url
    {
        [Key]
        public Guid Id { get; set; }

        [Required] 
        [Url] 
        public string LongUrl { get; set; }

        [Required]
        public string ShortUrl { get; set; }    
    }
}
