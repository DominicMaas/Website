using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Website.Models;

namespace Website.Services;

/// <summary>
///     Service helpers for Soundbyte legacy code
/// </summary>
public class SoundByteAuthenticationService
{
    public async Task<SoundByteGetTokenResponse> GetTokenAsync(string clientId, string clientSecret, string redirectUri, string code, string oAuthPath, string grantType, CancellationToken cancellationToken)
    {
        try
        {
            // Start the request
            using var client = new HttpClient(new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
            });

            // User agent
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("soundbyte-legacy-wrapper", "1.0.0"));

            // List of params needed to oauth2 tokens
            var parameters = new Dictionary<string, string>
            {
                { "client_id", clientId },
                { "client_secret", clientSecret },
                { "grant_type", grantType },
                { "redirect_uri", redirectUri },
            };

            // Add the correct extra param
            switch (grantType)
            {
                case "authorization_code":
                    parameters.Add("code", code);
                    break;

                case "refresh_token":
                    parameters.Add("refresh_token", code);
                    break;
            }

            // Encode the params
            var encodedContent = new FormUrlEncodedContent(parameters);

            // Post the respected API
            using var postQuery = await client.PostAsync(oAuthPath, encodedContent, cancellationToken);

            // If successful
            if (postQuery.IsSuccessStatusCode)
            {
                // Get the stream
                await using var stream = await postQuery.Content.ReadAsStreamAsync(cancellationToken);

                // Get the class from the json
                var response = await JsonSerializer.DeserializeAsync<SoundByteOAuthToken>(stream, cancellationToken: cancellationToken);

                // Return the login token
                return new SoundByteGetTokenResponse
                {
                    Successful = true,
                    ErrorMessage = null,
                    AccessToken = response?.AccessToken,
                    RefreshToken = response?.RefreshToken
                };
            }
            else
            {
                // Return the reason from the web server
                var reason = await postQuery.Content.ReadAsStringAsync(cancellationToken);
                return new SoundByteGetTokenResponse
                {
                    Successful = false,
                    ErrorMessage = reason.Trim('{', '}').Replace("\"", "")
                };
            }
        }
        catch (Exception ex)
        {
            return new SoundByteGetTokenResponse
            {
                Successful = false,
                ErrorMessage = ex.Message
            };
        }
    }
}
