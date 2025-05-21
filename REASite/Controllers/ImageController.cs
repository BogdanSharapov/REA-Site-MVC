using ImageStorage;
using Microsoft.AspNetCore.Mvc;

public class ImageController : Controller
{
    private readonly IImageService _imageService;

    public ImageController(IImageService imageService)
    {
        _imageService = imageService;
    }

    // Основной метод для получения изображений
    [ResponseCache(Duration = 3600)]
    [HttpGet]
    public async Task<IActionResult> Get(string imageId)
    {
        if (string.IsNullOrEmpty(imageId))
            return BadRequest("Image ID is required");

        try
        {
            var imageData = await _imageService.GetImageAsync(imageId);
            return imageData != null
                ? File(imageData, "image/jpeg")
                : NotFound();
        }
        catch
        {
            return StatusCode(500);
        }
    }
}