namespace AppCrudCore.Models.ViewModels.Home
{
    public class HomeCategoriaVM
    {
        public string CategoriaNombre { get; set; } = "";
        public List<Libro> Libros { get; set; } = new();
    }
}
