using System.Reactive.Linq;
using Minio;
using Minio.ApiEndpoints;
using Minio.DataModel.Args;
using Minio.Exceptions;
using StartupsApi.Interfaces;

namespace StartupsApi;

public class MinioService : IMinioService
{
    private readonly IMinioClient _minioClient;

    public MinioService(IMinioClient minioClient)
    {
        _minioClient = minioClient;
    }

    public async Task RemoveObject(string bucketName, string objectName)
    {
        var args = new RemoveObjectArgs()
            .WithBucket(bucketName)
            .WithObject(objectName);
        await _minioClient.RemoveObjectAsync(args);
    }

    public async IAsyncEnumerable<string> GetImageUrls(string bucketName)
    {
        var objects = _minioClient.ListObjectsEnumAsync(new ListObjectsArgs()
            .WithBucket(bucketName));
        await foreach (var obj in objects) yield return await GetImageUrl(bucketName, obj.Key);
    }

    public async Task RemoveBucket(string bucketName)
    {
        var found = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
        if (found)
        {
            var objects = _minioClient.ListObjectsAsync(new ListObjectsArgs().WithBucket(bucketName));
            foreach (var obj in objects) await RemoveObject(bucketName, obj.Key);
            await _minioClient.RemoveBucketAsync(new RemoveBucketArgs().WithBucket(bucketName));
        }
    }

    public async Task<string> GetImageUrl(string bucketName, string imageName)
    {
        return await _minioClient.PresignedGetObjectAsync(new PresignedGetObjectArgs()
            .WithBucket(bucketName)
            .WithObject(imageName)
            .WithExpiry(1000)
        );
    }

    public async Task AddImage(IFormFile file, string bucketName, string objectNmae)
    {
        try
        {
            var beArgs = new BucketExistsArgs()
                .WithBucket(bucketName);
            var found = await _minioClient.BucketExistsAsync(beArgs).ConfigureAwait(false);
            if (!found)
            {
                var mbArgs = new MakeBucketArgs()
                    .WithBucket(bucketName);
                await _minioClient.MakeBucketAsync(mbArgs).ConfigureAwait(false);

                var policyJson = $@"
                   {{
          ""Version"": ""2012-10-17"",
          ""Statement"": [
            {{
              ""Action"": [
                ""s3:GetObject""
              ],
              ""Effect"": ""Allow"",
              ""Principal"": {{
                ""AWS"": [
                  ""*""
                ]
              }},
              ""Resource"": [
                ""arn:aws:s3:::{bucketName}/*""
              ],
              ""Sid"": """"
            }}
          ]
        }}";

                await _minioClient.SetPolicyAsync(new SetPolicyArgs()
                    .WithBucket(bucketName)
                    .WithPolicy(policyJson));
            }


            var putObjectArgs = new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectNmae)
                .WithObjectSize(file.Length)
                .WithContentType("image/png")
                .WithStreamData(file.OpenReadStream());

            await _minioClient.PutObjectAsync(putObjectArgs).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            throw new MinioException("Error while adding image");
        }
    }

    public async Task AddImages(List<IFormFile> files, string bucketName)
    {
        try
        {
            foreach (var file in files) await AddImage(file, bucketName, file.GetHashCode().ToString());
        }
        catch (Exception e)
        {
            throw new MinioException("Error while adding images");
        }
    }
}