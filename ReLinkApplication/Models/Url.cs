using System.ComponentModel.DataAnnotations;

namespace ReLinkApplication.Models
{
    public class Url
    {
        [Key]
        public Guid Id { get; set; }

        [Url] 
        public string LongUrl { get; set; }

        public string ShortUrl { get; set; }    
    }
}
