﻿using ApiPeliculas.Models;

namespace ApiPeliculas.Repositorio.IRepositorio
{
    public interface IPeliculaRepositorio
    {
        //V1
        // Traer todas las Peliculas
        //ICollection<Pelicula> GetPeliculas();

        //V2 PAGINACION
        ICollection<Pelicula> GetPeliculas(int pageNumber, int pageSize);
        int GetTotalPeliculas();
        // Traer todas las Peliculas de una categoria
        ICollection<Pelicula> GetPeliculasEnCategoria(int catId);
        IEnumerable<Pelicula> BuscarPelicula(string nombre);
        // Traer solo 1 Pelicula
        Pelicula GetPelicula(int peliculaId);
        // Verificar si existe la Pelicula por id
        bool ExistePelicula(int id);
        // Verificar si existe la Pelicula por nombre
        bool ExistePelicula(string nombre);
        // Crear Pelicula
        bool CrearPelicula(Pelicula pelicula);
        // Actualizar una Pelicula
        bool ActualizarPelicula(Pelicula pelicula);
        // Borrar una Pelicula
        bool BorrarPelicula(Pelicula pelicula);
        bool Guardar();
    }
}
