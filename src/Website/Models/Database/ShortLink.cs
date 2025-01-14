#nullable disable

using System.ComponentModel.DataAnnotations.Schema;

namespace Website.Models.Database;

public class ShortLink
{
    public string Id { get; set; }

    public string RedirectLink { get; set; }

    public ICollection<ShortLinkHit> Hits { get; set; } = new List<ShortLinkHit>();

    [NotMapped] public int HitsCount { get; set; }
}
