using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;

namespace Website.Services;

public class R2
{
    private readonly IAmazonS3 _client;

    public R2(IConfiguration configuration)
    {
        var credentials = new BasicAWSCredentials(configuration["R2:ID"], configuration["R2:Secret"]);

        _client = new AmazonS3Client(credentials, new AmazonS3Config
        {
            ServiceURL = configuration["R2:URL"],
        });
    }

    public async Task UploadImageAsync(Stream image, string imageName, string type, CancellationToken cancellationToken = default)
    {
        var request = new PutObjectRequest
        {
            BucketName = "dominicmaas-images",
            Key = imageName,
            InputStream = image,
            ContentType = type,
            DisablePayloadSigning = true
        };

        var response = await _client.PutObjectAsync(request, cancellationToken);

        if (response.HttpStatusCode != System.Net.HttpStatusCode.OK && response.HttpStatusCode != System.Net.HttpStatusCode.Accepted)
        {
            throw new Exception("Upload to Cloudflare R2 failed");
        }
    }

    public async Task DeleteImageAsync(string imageName, CancellationToken cancellationToken = default)
    {
        var request = new DeleteObjectRequest
        {
            BucketName = "dominicmaas-images",
            Key = imageName
        };

        var response = await _client.DeleteObjectAsync(request, cancellationToken);

        if (response.HttpStatusCode != System.Net.HttpStatusCode.NoContent)
        {
            throw new Exception("Delete from Cloudflare R2 failed");
        }
    }
}
