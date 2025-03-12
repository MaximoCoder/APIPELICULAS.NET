using ApiPeliculas.Models;
using ApiPeliculas.Models.Dtos;
using ApiPeliculas.Repositorio.IRepositorio;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiPeliculas.Controllers.V2
{
    //[Authorize(Roles = "Admin")] // Para protegerlos de ser usados por alguien que no esta autenticado
    //[ResponseCache(Duration = 20)] // Para cache
    //[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)] // Para no permitir almacenar nada en cache
    // Utilizamos el perfil de cache
    [ResponseCache(CacheProfileName = "PorDefecto20Segundos")]
    [Route("api/v{version:apiVersion}/categorias")]
    [ApiController]

    // Especificamos la version de la API
    [ApiVersion("2.0")]
    public class CategoriasV2Controller : ControllerBase
    {
        private readonly ICategoriaRepositorio _ctRepo;
        private readonly IMapper _mapper;

        public CategoriasV2Controller(ICategoriaRepositorio ctRepo, IMapper mapper)
        {
            // Instancia al repositorio y al mapper
            _ctRepo = ctRepo;
            _mapper = mapper;
        }


    }
}
