using AppCrudCore.Models.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace AppCrudCore.Services.Implementations
{
    public class LibroImageService : ILibroImageService
    {
        private readonly IWebHostEnvironment _environment;

        public LibroImageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string?> GuardarImagenAsync(IFormFile? archivo)
        {
            if (archivo == null || archivo.Length == 0)
                return null;

            var carpeta = Path.Combine(
                _environment.WebRootPath,
                "images",
                "libros"
            );

            if (!Directory.Exists(carpeta))
                Directory.CreateDirectory(carpeta);

            var nombreArchivo = Guid.NewGuid() + ".webp";

            var rutaCompleta =
                Path.Combine(carpeta, nombreArchivo);

            using var image =
                await Image.LoadAsync(archivo.OpenReadStream());

            // RESIZE ECOMMERCE CARD
            image.Mutate(x => x
                .Resize(new ResizeOptions
                {
                    Size = new Size(400, 500),
                    Mode = ResizeMode.Crop
                }));

            // COMPRESIÓN WEBP
            await image.SaveAsync(
                rutaCompleta,
                new WebpEncoder
                {
                    Quality = 75
                });

            return "/images/libros/" + nombreArchivo;
        }

        public Task EliminarImagenAsync(string? rutaImagen)
        {
            if (string.IsNullOrEmpty(rutaImagen))
                return Task.CompletedTask;

            var rutaFisica = Path.Combine(
                _environment.WebRootPath,
                rutaImagen.TrimStart('/')
            );

            if (File.Exists(rutaFisica))
                File.Delete(rutaFisica);

            return Task.CompletedTask;
        }
    }
}