using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using Website.Common;

namespace Website.Controllers;

public class RSSController(DatabaseContext context) : Controller
{
    [ResponseCache(Duration = 1200)]
    [HttpGet("/feed/stream.xml")]
    public async Task<IActionResult> StreamRSSAsync()
    {
        var latestStream = await context.Streams.OrderByDescending(x => x.Posted).FirstOrDefaultAsync();

        var feed = BuildBasicFeed("Stream", "Quick thoughts and ideas", new("https://dominicmaas.co.nz/feed/stream.xml"), latestStream?.Posted ?? default);
        var items = new List<SyndicationItem>();


        var streams = await context.Streams.OrderByDescending(x => x.Posted).Take(20).ToListAsync();
        foreach (var stream in streams)
        {
            items.Add(new SyndicationItem(stream.Title, stream.Content, new Uri($"https://dominicmaas.co.nz/stream/{stream.Id}"), stream.Id.ToString(), stream.Posted));
        }

        feed.Items = items;

        return BuildSyndicationFeed(feed);
    }

    //[ResponseCache(Duration = 1200)]
    //[HttpGet("/feed/blog.xml")]
    //public IActionResult BlogRSS()
    //{
    //    var feed = BuildBasicFeed("Blog", "Long form posts or structured content", new("https://dominicmaas.co.nz/feed/blog.xml"));

    //    var testStream = new SyndicationItem("This is a test blog", "This is the content", new Uri("https://dominicmaas.co.nz/blog/123"), "123", DateTimeOffset.Now);
    //    feed.Items = new[] { testStream };

    //    return BuildSyndicationFeed(feed);
    //}

    private static SyndicationFeed BuildBasicFeed(string name, string description, Uri url, DateTime lastUpdated)
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
            Encoding = Encoding.UTF8,
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
