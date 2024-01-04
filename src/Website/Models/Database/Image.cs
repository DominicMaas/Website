using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Website.Models.Database;

/// <summary>
///     Represents an image on the website. These images can be embedded within streams, or
///     on the gallery section. Eventually we will support images in posts as well.
/// </summary>
public class Image
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public DateTime DateUploaded { get; set; }

    public DateTime? DateTaken { get; set; }

    public string R2Url { get; set; } = default!;

    public string? Description { get; set; }

    public List<StreamPost> Streams { get; set; } = [];

    public List<Gallery> Galleries { get; set; } = [];
}
