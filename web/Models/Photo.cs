namespace Website.Models;

public class Photo
{
    public Photo(string thumbnailPath, string imagePath)
    {
        ThumbnailPath = thumbnailPath;
        ImagePath = imagePath;
    }

    public string ThumbnailPath { get; init; }
    public string ImagePath { get; init; }
}
