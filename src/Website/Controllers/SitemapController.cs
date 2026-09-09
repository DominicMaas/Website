using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Xml;
using Website.Common;

namespace Website.Controllers;

/// <summary>
///     Generates /sitemap.xml from the static page list below plus <see cref="BlogIndex" />,
///     so adding a blog post does not require remembering to hand edit a second file.
/// </summary>
public class SitemapController : Controller
{
    private const string BaseUrl = "https://dominicmaas.co.nz";

    /// <summary>Non blog pages worth indexing, with the priority to advertise for each.</summary>
    private static readonly (string Path, string Priority)[] StaticPages =
    [
        ("/", "1.00"),
        ("/windows-app-development", "0.95"),
        ("/projects", "0.90"),
        ("/projects/soundbyte", "0.70"),
        ("/projects/danny", "0.70"),
        ("/contact", "0.90"),
        ("/cv", "0.90"),
        ("/blog", "0.90"),
        ("/photography", "0.60"),
        ("/media", "0.60"),
        ("/style", "0.30"),
        ("/privacy", "0.30"),
    ];

    [ResponseCache(Duration = 3600)]
    [HttpGet("/sitemap.xml")]
    public IActionResult Sitemap()
    {
        var settings = new XmlWriterSettings
        {
            Encoding = new UTF8Encoding(false),
            Indent = true
        };

        using var stream = new MemoryStream();
        using (var writer = XmlWriter.Create(stream, settings))
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");

            foreach (var (path, priority) in StaticPages)
            {
                WriteUrl(writer, BaseUrl + path, priority, lastModified: null);
            }

            foreach (var post in BlogIndex.Recent)
            {
                WriteUrl(writer, post.Url, "0.80", post.Updated.ToString("yyyy-MM-dd"));
            }

            writer.WriteEndElement();
            writer.WriteEndDocument();
        }

        return File(stream.ToArray(), "application/xml; charset=utf-8");
    }

    private static void WriteUrl(XmlWriter writer, string location, string priority, string? lastModified)
    {
        writer.WriteStartElement("url");
        writer.WriteElementString("loc", location);

        if (lastModified is not null)
        {
            writer.WriteElementString("lastmod", lastModified);
        }

        writer.WriteElementString("priority", priority);
        writer.WriteEndElement();
    }
}
