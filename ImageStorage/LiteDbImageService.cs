using LiteDB;
using LiteDB.Async;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace ImageStorage
{
    public class LiteDbImageService : IImageService
    {
        private readonly LiteDatabase _db;
        private readonly ILogger<LiteDbImageService> _logger;

        public LiteDbImageService(LiteDatabase db, ILogger<LiteDbImageService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<string> SaveImageAsync(Stream imageStream, string fileName, string contentType)
        {
            if (imageStream == null || !imageStream.CanRead)
            {
                throw new ArgumentException("Недопустимый поток изображения.");
            }

            using (var image = Image.Load(imageStream))
            {
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(800, 600),
                    Mode = ResizeMode.Max
                }));

                using (var memoryStream = new MemoryStream())
                {
                    IImageEncoder encoder = contentType.Contains("jpeg") ? new JpegEncoder() : new PngEncoder();
                    image.Save(memoryStream, encoder);
                    memoryStream.Position = 0;

                    var storage = _db.GetStorage<string>("images");
                    var imageId = Guid.NewGuid().ToString();
                    await Task.Run(() => storage.Upload(imageId, fileName, memoryStream));
                    return imageId;
                }
            }
        }

        public async Task<byte[]> GetImageAsync(string imageId)
        {
            try
            {
                var storage = _db.GetStorage<string>("images");
                if (await Task.Run(() => storage.Exists(imageId)))
                {
                    var memoryStream = new MemoryStream();
                    await Task.Run(() => storage.Download(imageId, memoryStream));
                    return memoryStream.ToArray();
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка в LiteDB: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteImageAsync(string imageId)
        {
            var storage = _db.GetStorage<string>("images");
            await Task.Run(() => storage.Delete(imageId));
        }
    }
}