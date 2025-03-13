using ApiPeliculas.Models;
using ApiPeliculas.Models.Dtos;
using ApiPeliculas.Repositorio.IRepositorio;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace ApiPeliculas.Controllers.V1
{
    [Route("api/v{version:apiVersion}/peliculas")]
    [ApiController]
    // Especificamos la version de la API
    [ApiVersion("1.0")]
    //[ApiVersion("2.0")]
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
        [AllowAnonymous]
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
        [AllowAnonymous]
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
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(PeliculaDto))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult CrearPelicula([FromForm] CrearPeliculaDto crearPeliculaDto)
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
            //if (!_pelRepo.CrearPelicula(pelicula))
            //{
            //    ModelState.AddModelError("", $"Algo salio mal guardando el registro{pelicula.Nombre}");
            //    return StatusCode(404, ModelState);
            //}

            //Subida de imagen
            if(crearPeliculaDto.Imagen != null)
            {
                // Construimos el nombre del archivo
                string nombreArchivo = pelicula.Id + System.Guid.NewGuid().ToString() + Path.GetExtension(crearPeliculaDto.Imagen.FileName);
                string rutaArchivo = @"wwwroot\ImagenesPeliculas\" + nombreArchivo;

                var ubicacionDirectorio = Path.Combine(Directory.GetCurrentDirectory(), rutaArchivo);

                FileInfo file = new FileInfo(ubicacionDirectorio);
                if (file.Exists) { 
                    file.Delete();
                }

                using(var fileStream = new FileStream(ubicacionDirectorio, FileMode.Create))
                {
                    crearPeliculaDto.Imagen.CopyTo(fileStream);
                }

                var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                pelicula.RutaImagen = baseUrl + "/ImagenesPeliculas/" + nombreArchivo;
                pelicula.RutaLocalImagen = rutaArchivo;
            }
            else
            {
                // No hay imagen
                pelicula.RutaImagen = "https://placehold.co/600x400";
            }
            _pelRepo.CrearPelicula(pelicula);
            return CreatedAtRoute("GetPelicula", new { peliculaId = pelicula.Id }, pelicula);
        }

        // Actualizar una pelicula parcialmente PATCH
        [Authorize(Roles = "Admin")]
        [HttpPatch("{peliculaId:int}", Name = "ActualizarPatchPelicula")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ActualizarPatchPelicula(int peliculaId, [FromForm] ActualizarPeliculaDto actualizarPeliculaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (actualizarPeliculaDto == null || peliculaId != actualizarPeliculaDto.Id)
            {
                return BadRequest(ModelState);
            }

            var peliculaExistente = _pelRepo.GetPelicula(peliculaId);
            if (peliculaExistente == null)
            {
                return NotFound($"No se encontro la película con ID {peliculaId}");
            }

            var pelicula = _mapper.Map<Pelicula>(actualizarPeliculaDto);

            //if (!_pelRepo.ActualizarPelicula(pelicula))
            //{
            //    ModelState.AddModelError("", $"Algo salio mal actualizando el registro{pelicula.Nombre}");
            //    return StatusCode(500, ModelState);
            //}

            //Subida de Archivo
            if (actualizarPeliculaDto.Imagen != null)
            {
                string nombreArchivo = pelicula.Id + System.Guid.NewGuid().ToString() + Path.GetExtension(actualizarPeliculaDto.Imagen.FileName);
                string rutaArchivo = @"wwwroot\ImagenesPeliculas\" + nombreArchivo;

                var ubicacionDirectorio = Path.Combine(Directory.GetCurrentDirectory(), rutaArchivo);

                FileInfo file = new FileInfo(ubicacionDirectorio);

                if (file.Exists)
                {
                    file.Delete();
                }

                using (var fileStream = new FileStream(ubicacionDirectorio, FileMode.Create))
                {
                    actualizarPeliculaDto.Imagen.CopyTo(fileStream);
                }

                var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                pelicula.RutaImagen = baseUrl + "/ImagenesPeliculas/" + nombreArchivo;
                pelicula.RutaLocalImagen = rutaArchivo;
            }
            else
            {
                pelicula.RutaImagen = "https://placehold.co/600x400";
            }

            _pelRepo.ActualizarPelicula(pelicula);
            return NoContent();

        }

        // Actualizar una PELICULA CON PUT
        [Authorize(Roles = "Admin")]
        [HttpPut("{peliculaId:int}", Name = "ActualizarPutPelicula")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ActualizarPutPelicula(int peliculaId, [FromForm] ActualizarPeliculaDto peliculaDto)
        {
            // Validar si el modelo es inválido
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validar si la película enviada es null o el ID no coincide
            if (peliculaDto == null || peliculaId != peliculaDto.Id)
            {
                return BadRequest(ModelState);
            }

            var peliculaExistente = _pelRepo.GetPelicula(peliculaId);
            if (peliculaExistente == null)
            {
                return NotFound($"La película con ID {peliculaId} no existe.");
            }

            // Mapear los datos actualizados
            var pelicula = _mapper.Map<Pelicula>(peliculaDto);

            // Manejar la subida de la imagen si se proporciona una nueva
            if (peliculaDto.Imagen != null)
            {
                // Crear nombre único para el archivo
                string nombreArchivo = pelicula.Id + System.Guid.NewGuid().ToString() + Path.GetExtension(peliculaDto.Imagen.FileName);
                string rutaArchivo = @"wwwroot\ImagenesPeliculas\" + nombreArchivo;

                var ubicacionDirectorio = Path.Combine(Directory.GetCurrentDirectory(), rutaArchivo);

                // Eliminar la imagen anterior si existe
                FileInfo file = new FileInfo(ubicacionDirectorio);
                if (file.Exists)
                {
                    file.Delete();
                }

                // Guardar la nueva imagen
                using (var fileStream = new FileStream(ubicacionDirectorio, FileMode.Create))
                {
                    peliculaDto.Imagen.CopyTo(fileStream);
                }

                // Construir la URL de la imagen
                var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                pelicula.RutaImagen = baseUrl + "/ImagenesPeliculas/" + nombreArchivo;
                pelicula.RutaLocalImagen = rutaArchivo;
            }
            else
            {
                // Mantener la imagen existente o asignar una por defecto
                pelicula.RutaImagen = peliculaExistente.RutaImagen ?? "https://placehold.co/600x400";
            }

            // Intentar actualizar la película
            if (!_pelRepo.ActualizarPelicula(pelicula))
            {
                ModelState.AddModelError("", $"Algo salió mal actualizando el registro {pelicula.Nombre}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        // Eliminar una pelicula con DELETE
        [Authorize(Roles = "Admin")]
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
        [AllowAnonymous]
        [HttpGet("GetPeliculasEnCategoria/{categoriaId:int}")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetPeliculasEnCategoria(int categoriaId)
        {
            var listaPeliculas = _pelRepo.GetPeliculasEnCategoria(categoriaId);
            // Validar si si tiene peliculas esa categoria
            if (listaPeliculas == null)
            {
                return NotFound();
            }
            // Caso contrario mostramos las peliculas
            var itemPelicula = new List<PeliculaDto>();
            foreach (var pelicula in listaPeliculas)
            {
                itemPelicula.Add(_mapper.Map<PeliculaDto>(pelicula));
            }
            return Ok(itemPelicula);
        }

        // Buscar pelicula por nombre o descripcion
        [AllowAnonymous]
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
                if (resultado.Any())
                {
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
