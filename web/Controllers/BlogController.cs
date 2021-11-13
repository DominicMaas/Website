using Microsoft.AspNetCore.Mvc;
using Website.Models;

namespace Website.Controllers;

public class BlogController : Controller
{
    private static readonly List<Post> Posts = new()
    {
        new Post
        {
            IsPublished = true,
            Published = new DateOnly(2021, 11, 13),
            Slug = "hello-world",
            Title = "Hello World",
            Description = "I built a blog! Kind of... (but it's not ready yet!). This is just a test post to test how markdown is rendered on this website."
        }
    };

    public IActionResult Index()
    {
        return View(Posts);
    }

    public IActionResult Post(string id)
    {
        var post = Posts.Find(x => x.Slug == id);
        if (post == null) return NotFound();

        return View(post);
    }
}
