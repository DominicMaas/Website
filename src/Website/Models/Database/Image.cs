using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Website.Models.Database;

/// <summary>
///     Represents an image on the website. These images can be embedded within blog posts,
///     or on the gallery section.
/// </summary>
public class Image
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public DateTime DateUploaded { get; set; }

    [DisplayName("Date Taken")]
    public DateTime? DateTaken { get; set; }

    public string? Description { get; set; }

    [NotMapped]
    public string Url => $"https://images.dominicmaas.co.nz/i/{Id}.jpg";
}
