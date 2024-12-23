using System.ComponentModel.DataAnnotations;

namespace ReLinkApplication.Models;

public class Url
{
    public Guid Id { get; set; }

    [Required]
    public string LongUrl { get; set; }

    [Required]
    public string ShortUrl { get; set; }    
}
