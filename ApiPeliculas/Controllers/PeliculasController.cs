using ApiPeliculas.Models;
using ApiPeliculas.Models.Dtos;
using ApiPeliculas.Repositorio.IRepositorio;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace ApiPeliculas.Controllers
{
    [Route("api/peliculas")]
    [ApiController]
    public class PeliculasController : ControllerBase
    {
        private readonly IPeliculaRepositorio _pelRepo;
        private readonly IMapper _mapper;

        public PeliculasController(IPeliculaRepositorio pelRepo, IMapper mapper)
        {
            // Instancia al repositorio y al mapper
            _pelRepo = pelRepo;
            _mapper = mapper;
        }

        // obtener Peliculas
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetPeliculas()
        {
            var listaPeliculas = _pelRepo.GetPeliculas();
            // Exponer el dto
            var listaPeliculasDto = new List<PeliculaDto>();
            // Mapeamos la lista del dto con la del modelo
            foreach (var lista in listaPeliculas)
            {
                listaPeliculasDto.Add(_mapper.Map<PeliculaDto>(lista));
            }
            // Devolvemos el dto
            return Ok(listaPeliculasDto);
        }

        // obtener 1 pelicula
        [HttpGet("{peliculaId:int}", Name = "GetPelicula")] // Aceptamos el parametro 
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetPelicula(int peliculaId)
        {
            var itemPelicula = _pelRepo.GetPelicula(peliculaId);

            //Validamos si es null
            if (itemPelicula == null)
            {
                return NotFound();
            }
            // Exponer el dto
            var itemPeliculaDto = _mapper.Map<PeliculaDto>(itemPelicula);

            // Devolvemos el dto
            return Ok(itemPeliculaDto);
        }

        // Crear una pelicula
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(PeliculaDto))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult CrearPelicula([FromBody] CrearPeliculaDto crearPeliculaDto)
        {
            //Validamos si el modelo no es valido
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validamos si es null
            if (crearPeliculaDto == null) { return BadRequest(ModelState); }

            // Validamos si ya existe esa categoria
            if (_pelRepo.ExistePelicula(crearPeliculaDto.Nombre))
            {
                ModelState.AddModelError("", "La pelicula ya existe");
                return StatusCode(404, ModelState);
            }

            var pelicula = _mapper.Map<Pelicula>(crearPeliculaDto);

            // Validamos si no se pudo crear entonces devolvemos error
            if (!_pelRepo.CrearPelicula(pelicula))
            {
                ModelState.AddModelError("", $"Algo salio mal guardando el registro{pelicula.Nombre}");
                return StatusCode(404, ModelState);
            }

            return CreatedAtRoute("GetPelicula", new { peliculaId = pelicula.Id }, pelicula);
        }

        // Actualizar una pelicula parcialmente PATCH
        [HttpPatch("{peliculaId:int}", Name = "ActualizarPatchPelicula")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ActualizarPatchPelicula(int peliculaId, [FromBody] PeliculaDto peliculaDto)
        {
            //Validamos si el modelo no es valido
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validamos si es null o si no existe el id
            if (peliculaDto == null || peliculaId != peliculaDto.Id) { return BadRequest(ModelState); }
            var peliculaExistente = _pelRepo.GetPelicula(peliculaId);
            if (peliculaExistente == null) { return NotFound($"La pelicula ingresada no existe{peliculaId}"); }

            var pelicula = _mapper.Map<Pelicula>(peliculaDto);

            // Validamos si no se pudo crear entonces devolvemos error
            if (!_pelRepo.ActualizarPelicula(pelicula))
            {
                ModelState.AddModelError("", $"Algo salio mal actualizando el registro{pelicula.Nombre}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        // Actualizar una PELICULA CON PUT
        [HttpPut("{peliculaId:int}", Name = "ActualizarPutPelicula")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ActualizarPutPelicula(int peliculaId, [FromBody] PeliculaDto peliculaDto)
        {
            //Validamos si el modelo no es valido
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validamos si es null o si no existe el id
            if (peliculaDto == null || peliculaId != peliculaDto.Id) { return BadRequest(ModelState); }
            var peliculaExistente = _pelRepo.GetPelicula(peliculaId);
            if (peliculaExistente == null) { return NotFound($"La pelicula ingresada no existe{peliculaId}"); }

            var pelicula = _mapper.Map<Pelicula>(peliculaDto);

            // Validamos si no se pudo crear entonces devolvemos error
            if (!_pelRepo.ActualizarPelicula(pelicula))
            {
                ModelState.AddModelError("", $"Algo salio mal actualizando el registro{pelicula.Nombre}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        // Eliminar una pelicula con DELETE
        [HttpDelete("{peliculaId:int}", Name = "BorrarPelicula")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult BorrarPelicula(int peliculaId)
        {
            // Validamos si existe la pelicula
            if (!_pelRepo.ExistePelicula(peliculaId))
            {
                return NotFound($"La pelicula ingresada no existe{peliculaId}");
            }

            var pelicula = _pelRepo.GetPelicula(peliculaId);

            // Validamos si no se pudo crear entonces devolvemos error
            if (!_pelRepo.BorrarPelicula(pelicula))
            {
                ModelState.AddModelError("", $"Algo salio mal borrando el registro{pelicula.Nombre}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        // Obtener la lista de peliculas de una categoria especifica
        [HttpGet("GetPeliculasEnCategoria/{categoriaId:int}")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetPeliculasEnCategoria(int categoriaId) {
            var listaPeliculas = _pelRepo.GetPeliculasEnCategoria(categoriaId);
            // Validar si si tiene peliculas esa categoria
            if(listaPeliculas == null)
            {
                return NotFound();
            }
            // Caso contrario mostramos las peliculas
            var itemPelicula = new List<PeliculaDto>();
            foreach(var pelicula in listaPeliculas)
            {
                itemPelicula.Add(_mapper.Map<PeliculaDto>(pelicula));
            }
            return Ok(itemPelicula);
        }

        // Buscar pelicula por nombre o descripcion
        [HttpGet("Buscar")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Buscar(string nombre)
        {
            try
            {
                var resultado = _pelRepo.BuscarPelicula(nombre);
                // Si encuentra algun resultado entonces lo devolvemos
                if (resultado.Any()) { 
                    return Ok(resultado);
                }
                // Caso contrario
                return NotFound();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error recuperando los datos");
            }
        }
    }
}
