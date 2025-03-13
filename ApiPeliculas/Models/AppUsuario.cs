using Microsoft.AspNetCore.Identity;

namespace ApiPeliculas.Models
{
    public class AppUsuario : IdentityUser
    {
        // Campo nuevo
        public string Nombre { get; set; }
    }
}
