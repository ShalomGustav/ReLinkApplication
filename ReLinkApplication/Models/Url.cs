using System.ComponentModel.DataAnnotations;

namespace ReLinkApplication.Models;

public class Url
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(2048)]
    public string LongUrl { get; set; }

    [Required]
    [MaxLength(100)]
    public string ShortUrl { get; set; }    
}
