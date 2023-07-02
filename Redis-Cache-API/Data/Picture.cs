using System.ComponentModel.DataAnnotations;

namespace Redis_Cache_API.Data;
public class Picture
{
    [Key]
    public int PictureId { get; set; }

    [Required]
    public string? Url { get; set; }

    public string? Name { get; set; }

    public string? AltText { get; set; }
}
