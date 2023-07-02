namespace Website.Models;

public class SoundByteGetTokenResponse
{
    public bool Successful { get; set; }

    public string? ErrorMessage { get; set; }

    public string? AccessToken { get; set; }

    public string? RefreshToken { get; set; }
}
