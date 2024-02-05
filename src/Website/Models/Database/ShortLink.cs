#nullable disable

namespace Website.Models.Database;

public class ShortLink
{
    public string Id { get; set; }

    public string RedirectLink { get; set; }

    public List<ShortLinkHit> Hits { get; set; }
}
