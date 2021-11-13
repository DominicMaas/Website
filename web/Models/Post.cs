namespace Website.Models;

/// <summary>
///     Represents a post within this website
/// </summary>
public class Post
{
    /// <summary>
    ///     The id / URL slug for this page. Once created, this shouldn't be changed.
    /// </summary>
    public string? Slug { get; set; }

    /// <summary>
    ///     If this post should appear in the recent posts list / be navigable
    /// </summary>
    public bool IsPublished { get; set; }

    /// <summary>
    ///     The date this post was published
    /// </summary>
    public DateOnly? Published { get; set; }

    /// <summary>
    ///     The title of the blog post
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    ///     The short description of the blog post
    /// </summary>
    public string? Description { get; set; }
}
