using Microsoft.AspNetCore.Mvc;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using Website.Common;

namespace Website.Controllers;

public class RSSController : Controller
{
    [ResponseCache(Duration = 1200)]
    [HttpGet("/feed/blog.xml")]
    public IActionResult BlogRSS()
    {
        var posts = BlogIndex.Recent.ToList();

        var feed = BuildBasicFeed(
            "Blog",
            "Technical write-ups on Windows desktop development, embedded programming and graphics.",
            new Uri("https://dominicmaas.co.nz/feed/blog.xml"),
            posts.Count > 0 ? posts.Max(x => x.UpdatedAt) : DateTimeOffset.MinValue);

        feed.Items = posts.Select(post => new SyndicationItem(
            post.Title,
            post.Summary,
            new Uri(post.Url),
            post.Url,
            post.UpdatedAt)
        {
            PublishDate = post.PublishedAt
        });

        return BuildSyndicationFeed(feed);
    }

    private static SyndicationFeed BuildBasicFeed(string name, string description, Uri url, DateTimeOffset lastUpdated)
    {
        var feed = new SyndicationFeed($"Dominic Maas - {name}", description, url);
        feed.Authors.Add(new SyndicationPerson("contact@dominicmaas.co.nz", "Dominic Maas", "https://dominicmaas.co.nz"));
        feed.Copyright = new TextSyndicationContent($"{DateTime.Now.Year} Dominic Maas");
        feed.ImageUrl = new Uri("https://dominicmaas.co.nz/favicon.ico");
        feed.LastUpdatedTime = lastUpdated;

        return feed;
    }

    private FileContentResult BuildSyndicationFeed(SyndicationFeed feed)
    {
        var settings = new XmlWriterSettings
        {
            Encoding = new UTF8Encoding(false),
            NewLineHandling = NewLineHandling.Entitize,
            NewLineOnAttributes = true,
            Indent = true
        };

        using var stream = new MemoryStream();
        using var xmlWriter = XmlWriter.Create(stream, settings);

        var rssFormatter = new Rss20FeedFormatter(feed, false);
        rssFormatter.WriteTo(xmlWriter);
        xmlWriter.Flush();

        return File(stream.ToArray(), "application/rss+xml; charset=utf-8");
    }
}
