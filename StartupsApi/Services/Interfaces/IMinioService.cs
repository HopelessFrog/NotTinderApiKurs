namespace StartupsApi.Interfaces;

public interface IMinioService
{
    Task AddImages(List<IFormFile> files, string bucketName);
    Task AddImage(IFormFile image, string bucketName, string objectName);
    Task<string> GetImageUrl(string bucketName, string imageName);
    Task RemoveBucket(string bucketName);
    Task RemoveObject(string bucketName, string objectName);

    IAsyncEnumerable<string> GetImageUrls(string bucketName);
}