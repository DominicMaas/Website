namespace Website.Models
{
    /// <summary>
    ///     Represents a post within this website
    /// </summary>
    public class Post
    {
        /// <summary>
        ///     The id / URL slug for this page. Once created, this shouldn't be changed.
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        ///     If this post should appear in the recent posts list / be navigable
        /// </summary>
        public bool IsPublished { get; set; }

        /// <summary>
        ///     Where in the wwwroot is the markdown file for this blog post located
        /// </summary>
        public string MarkdownFile { get; set; }
    }
}
