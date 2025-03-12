using ApiPeliculas.Models;

namespace ApiPeliculas.Repositorio.IRepositorio
{
    public interface ICategoriaRepositorio
    {
        // Traer todas las categorias
        ICollection<Categoria> GetCategorias();
        // Traer solo 1 categoria
        Categoria GetCategoria(int CategoriaId);
        // Verificar si existe la categoria por id
        bool ExisteCategoria(int id);
        // Verificar si existe la categoria por nombre
        bool ExisteCategoria(string nombre);
        // Crear categoria
        bool CrearCategoria(Categoria categoria);
        // Actualizar una categoria
        bool ActualizarCategoria(Categoria categoria);
        // Borrar una categoria
        bool BorrarCategoria(Categoria categoria);
        bool Guardar();
    }
}
