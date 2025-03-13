using ApiPeliculas.Models;
using ApiPeliculas.Models.Dtos;

namespace ApiPeliculas.Repositorio.IRepositorio
{
    public interface IUsuarioRepositorio
    {
        // Obtener todos los usuarios
        ICollection<AppUsuario> GetUsuarios();
        // Obtener un usuario
        AppUsuario GetUsuario(string usuarioId);
        // Validar si es unico
        bool IsUniqueUser(string usuario);

        // Version sin identity
        // Login
        //Task<UsuarioLoginRespuestaDto> Login(UsuarioLoginDto usuarioLoginDto);
        // Registro
        //Task<Usuario> Registro(UsuarioRegistroDto usuarioRegistroDto);

        // Version con identity
        Task<UsuarioLoginRespuestaDto> Login(UsuarioLoginDto usuarioLoginDto);
        Task<UsuarioDatosDto> Registro(UsuarioRegistroDto usuarioRegistroDto);
    }
}
