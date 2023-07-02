using System.Text.Json.Serialization;

namespace Website.Models;

public class SoundByteOAuthToken
{
    [JsonPropertyName("access_token")]
    public string? AccessToken { get; set; }

    [JsonPropertyName("token_type")]
    public string? TokenType { get; set; }

    [JsonPropertyName("expire_time")]
    public string? ExpireTime { get; set; }

    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; set; }
}
