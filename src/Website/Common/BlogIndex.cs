namespace Website.Common;

/// <summary>
///     A single entry in the blog. Each post is a hand written Razor page under /Pages/Blog,
///     this record is the metadata that drives the index page, the RSS feed and the sitemap.
/// </summary>
/// <param name="Slug">The URL segment, matching the Razor page file name (slugified).</param>
/// <param name="Title">Title as shown on the index and in the feed.</param>
/// <param name="Summary">One or two sentences describing the post.</param>
/// <param name="Published">The date the post first went live.</param>
/// <param name="Updated">The date the post was last meaningfully changed.</param>
public record BlogPost(string Slug, string Title, string Summary, DateOnly Published, DateOnly Updated)
{
    public string Url => $"https://dominicmaas.co.nz/blog/{Slug}";

    public DateTimeOffset UpdatedAt => new(Updated.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);

    public DateTimeOffset PublishedAt => new(Published.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
}

/// <summary>
///     The single source of truth for what lives in the blog. Add a new post here after
///     creating its Razor page and it will appear on /blog, in /feed/blog.xml and in /sitemap.xml.
///
///     This is for technical writing only. Dated posts someone might search for. Living
///     pages such as /pc, /running and /photography are standalone and linked from the footer.
/// </summary>
public static class BlogIndex
{
    public static readonly IReadOnlyList<BlogPost> Posts =
    [
        new("password-manager",
            "Building a Password Manager",
            "A password manager written for my third COMPX518 assignment, and the design decisions behind it.",
            new DateOnly(2023, 7, 3), new DateOnly(2026, 4, 13)),

        new("pico-filesystem-littlefs",
            "Raspberry Pi Pico File System via LittleFS",
            "How to use the built in ROM of the Raspberry Pi Pico as a file system using LittleFS.",
            new DateOnly(2023, 7, 3), new DateOnly(2023, 7, 3)),
    ];

    /// <summary>Posts newest first.</summary>
    public static IEnumerable<BlogPost> Recent => Posts.OrderByDescending(x => x.Published);
}
