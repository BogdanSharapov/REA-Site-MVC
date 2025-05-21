namespace ImageStorage
{
    public interface IImageService
    {
        Task<string> SaveImageAsync(Stream imageStream, string fileName, string contentType);
        Task<byte[]> GetImageAsync(string imageId);
        Task DeleteImageAsync(string imageId);
    }
}
