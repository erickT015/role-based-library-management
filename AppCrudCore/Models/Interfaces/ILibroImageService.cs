namespace AppCrudCore.Models.Interfaces
{
    public interface ILibroImageService
    {
        Task<string?> GuardarImagenAsync(IFormFile? archivo);

        Task EliminarImagenAsync(string? rutaImagen);
    }
}
