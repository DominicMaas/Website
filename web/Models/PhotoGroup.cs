namespace Website.Models;

public class PhotoGroup
{
    public PhotoGroup(string title, string? description, List<Photo> photos)
    {
        Title = title;
        Description = description;
        Photos = photos;
    }

    public string Title { get; init; }
    public string? Description { get; init; }
    public List<Photo> Photos { get; init; }
}
