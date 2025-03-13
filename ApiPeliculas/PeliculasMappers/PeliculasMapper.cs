using ApiPeliculas.Models;
using ApiPeliculas.Models.Dtos;
using AutoMapper;

namespace ApiPeliculas.PeliculasMapper
{
    public class PeliculasMapper: Profile
    {
        public PeliculasMapper()
        {
            // Categoria
            CreateMap<Categoria, CategoriaDto>().ReverseMap();
            CreateMap<Categoria, CrearCategoriaDto>().ReverseMap();
            // Pelicula
            CreateMap<Pelicula, PeliculaDto>().ReverseMap();
            CreateMap<Pelicula, CrearPeliculaDto>().ReverseMap();
            CreateMap<Pelicula, ActualizarPeliculaDto>().ReverseMap();
            // Usuario Sin identity
            //CreateMap<Usuario, UsuarioDto>().ReverseMap();
            //CreateMap<Usuario, UsuarioRegistroDto>().ReverseMap();

            // Usuario con Identity
            CreateMap<AppUsuario, UsuarioDatosDto>().ReverseMap();
            CreateMap<AppUsuario, UsuarioDto>().ReverseMap();
        }
    }
}
