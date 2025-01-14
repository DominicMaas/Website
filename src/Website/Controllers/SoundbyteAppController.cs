using Microsoft.AspNetCore.Mvc;
using Website.Services;

namespace Website.Controllers;

/// <summary>
///     A controller used to handle soundbyte external login providers (youtube / soundcloud)
/// </summary>
[Produces("application/json")]
[Route("api/soundbyte")]
[ApiController]
public class SoundbyteAppController(IConfiguration config, SoundByteAuthenticationService soundByteAuthenticationService) : Controller
{
    // These fields are already public within the soundbyte app and code (the client secret is extracted)

    public const string SoundCloudClientId = "gU5Rw9VDiPPA4OcDlC8VVcb19sHDZFTT";
    public const string SoundCloudConnectUrl = "https://api.soundcloud.com/oauth2/token";
    public const string SoundCloudRedirecturl = "http://localhost/soundbyte";

    public const string YouTubeClientId = "82331099881-4sf24bfns9f7p8b9o5ovvi8lveolru8i.apps.googleusercontent.com";
    public const string YouTubeConnectUrl = "https://accounts.google.com/o/oauth2/token";
    public const string YouTubeRedirecturl = "http://localhost/soundbyte";

    [HttpPost("auth/youtube")]
    public async Task<IActionResult> AuthYouTube([FromForm] string code, CancellationToken cancellationToken)
    {
        // Ensure valid parameters
        if (string.IsNullOrEmpty(code))
        {
            return new JsonResult(new
            {
                successful = false,
                error_message = "Missing 'code' parameter"
            });
        }

        var secret = config["SoundByteAuth:YouTubeClientSecret"];
        if (string.IsNullOrEmpty(secret))
        {
            return new JsonResult(new
            {
                successful = false,
                error_message = "The SoundByte Legacy service (hosted at https://dominicmaas.co.nz) has not been configured correctly!"
            });
        }

        // Perform the actual request
        var result = await soundByteAuthenticationService.GetTokenAsync(YouTubeClientId, secret, YouTubeRedirecturl,
            code, YouTubeConnectUrl, "authorization_code", cancellationToken);

        return new JsonResult(new
        {
            successful = result.Successful,
            error_message = result.ErrorMessage,
            login_token = new
            {
                access_token = result.AccessToken,
                refresh_token = result.RefreshToken
            }
        });
    }

    [HttpPost("auth/soundcloud")]
    public async Task<IActionResult> AuthSoundCloud([FromForm] string code, CancellationToken cancellationToken)
    {
        // Ensure valid parameters
        if (string.IsNullOrEmpty(code))
        {
            return new JsonResult(new
            {
                successful = false,
                error_message = "Missing 'code' parameter"
            });
        }

        var secret = config["SoundByteAuth:SoundCloudClientSecret"];
        if (string.IsNullOrEmpty(secret))
        {
            return new JsonResult(new
            {
                successful = false,
                error_message = "The SoundByte Legacy service (hosted at https://dominicmaas.co.nz) has not been configured correctly!"
            });
        }

        // Perform the actual request
        var result = await soundByteAuthenticationService.GetTokenAsync(SoundCloudClientId, secret, SoundCloudRedirecturl,
           code, SoundCloudConnectUrl, "authorization_code", cancellationToken);

        return new JsonResult(new
        {
            successful = result.Successful,
            error_message = result.ErrorMessage,
            login_token = new
            {
                access_token = result.AccessToken,
                refresh_token = result.RefreshToken
            }
        });
    }

    [HttpPost("refresh-auth/youtube")]
    public async Task<IActionResult> RefreshAuthYouTube([FromForm(Name = "refresh_token")] string refreshToken, CancellationToken cancellationToken)
    {
        // Ensure valid parameters
        if (string.IsNullOrEmpty(refreshToken))
        {
            return new JsonResult(new
            {
                successful = false,
                error_message = "Missing 'refresh_token' parameter"
            });
        }

        var secret = config["SoundByteAuth:YouTubeClientSecret"];
        if (string.IsNullOrEmpty(secret))
        {
            return new JsonResult(new
            {
                successful = false,
                error_message = "The SoundByte Legacy service (hosted at https://dominicmaas.co.nz) has not been configured correctly!"
            });
        }

        // Perform the actual request
        var result = await soundByteAuthenticationService.GetTokenAsync(YouTubeClientId, secret, YouTubeRedirecturl,
            refreshToken, YouTubeConnectUrl, "refresh_token", cancellationToken);

        return new JsonResult(new
        {
            successful = result.Successful,
            error_message = result.ErrorMessage,
            login_token = new
            {
                access_token = result.AccessToken,
                refresh_token = result.RefreshToken
            }
        });
    }

    [HttpPost("refresh-auth/soundcloud")]
    public async Task<IActionResult> RefreshAuthSoundCloud([FromForm(Name = "refresh_token")] string refreshToken, CancellationToken cancellationToken)
    {
        // Ensure valid parameters
        if (string.IsNullOrEmpty(refreshToken))
        {
            return new JsonResult(new
            {
                successful = false,
                error_message = "Missing 'refresh_token' parameter"
            });
        }

        var secret = config["SoundByteAuth:SoundCloudClientSecret"];
        if (string.IsNullOrEmpty(secret))
        {
            return new JsonResult(new
            {
                successful = false,
                error_message = "The SoundByte Legacy service (hosted at https://dominicmaas.co.nz) has not been configured correctly!"
            });
        }

        // Perform the actual request
        var result = await soundByteAuthenticationService.GetTokenAsync(SoundCloudClientId, secret, SoundCloudRedirecturl,
           refreshToken, SoundCloudConnectUrl, "refresh_token", cancellationToken);

        return new JsonResult(new
        {
            successful = result.Successful,
            error_message = result.ErrorMessage,
            login_token = new
            {
                access_token = result.AccessToken,
                refresh_token = result.RefreshToken
            }
        });
    }
}
