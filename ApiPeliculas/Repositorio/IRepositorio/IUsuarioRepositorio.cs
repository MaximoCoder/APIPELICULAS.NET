using ApiPeliculas.Models;
using ApiPeliculas.Models.Dtos;

namespace ApiPeliculas.Repositorio.IRepositorio
{
    public interface IUsuarioRepositorio
    {
        // Obtener todos los usuarios
        ICollection<Usuario> GetUsuarios();
        // Obtener un usuario
        Usuario GetUsuario(int usuarioId);
        // Validar si es unico
        bool IsUniqueUser(string usuario);
       // Login
        Task<UsuarioLoginRespuestaDto> Login(UsuarioLoginDto usuarioLoginDto);
        // Registro
        Task<Usuario> Registro(UsuarioRegistroDto usuarioRegistroDto);
    }
}
