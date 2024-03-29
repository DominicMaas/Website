using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Website.Models.Database;

public class StreamPost
{
    /// <summary>
    ///     A unique id for this stream item
    /// </summary>
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    ///     The date and time this stream item was posted
    /// </summary>
    public DateTime Posted { get; set; }

    /// <summary>
    ///     Simple title
    /// </summary>
    public string Title { get; set; } = default!;

    /// <summary>
    ///     Content of the stream
    /// </summary>
    public string Content { get; set; } = default!;

    /// <summary>
    ///     A list of optionally attached images to this stream
    /// </summary>
    public List<Image> AttachedImages { get; set; } = [];
}
