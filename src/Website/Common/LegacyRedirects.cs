namespace Website.Common;

/// <summary>
///     Permanent redirects for URLs that have moved. Keeping these around preserves any
///     inbound links and search ranking the old URLs picked up.
/// </summary>
public static class LegacyRedirects
{
    /// <summary>Old path (lower case, no trailing slash) mapped to its replacement.</summary>
    private static readonly Dictionary<string, string> Map = new(StringComparer.OrdinalIgnoreCase)
    {
        // The "pages" section became the blog in 2026.
        ["/pages"] = "/blog",
        ["/pages/pc"] = "/pc",
        ["/pages/password-manager"] = "/blog/password-manager",
        ["/pages/pico-filesystem-littlefs"] = "/blog/pico-filesystem-littlefs",
        ["/pages/running"] = "/running",

        // This page only ever said "the content moved", so send visitors straight there.
        ["/soundbyte-postmortem"] = "/projects/soundbyte",

        // Old blog posts lived under /pages/blog, which was never real content.
        ["/pages/blog"] = "/blog",
    };

    public static bool TryResolve(string path, out string destination)
    {
        var normalised = path.TrimEnd('/');

        if (normalised.Length == 0)
        {
            normalised = "/";
        }

        return Map.TryGetValue(normalised, out destination!);
    }
}
