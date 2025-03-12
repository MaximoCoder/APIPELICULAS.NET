using ApiPeliculas.Models;
using ApiPeliculas.Models.Dtos;
using ApiPeliculas.Repositorio.IRepositorio;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiPeliculas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly ICategoriaRepositorio _ctRepo;
        private readonly IMapper _mapper;

        public CategoriasController(ICategoriaRepositorio ctRepo, IMapper mapper)
        {
            // Instancia al repositorio y al mapper
            _ctRepo = ctRepo;
            _mapper = mapper;
        }

        // obtener categorias
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetCategorias() { 
            var listaCategorias = _ctRepo.GetCategorias();
            // Exponer el dto
            var listaCategoriasDto = new List<CategoriaDto>();
            // Mapeamos la lista del dto con la del modelo
            foreach (var lista in listaCategorias) { 
                listaCategoriasDto.Add(_mapper.Map<CategoriaDto>(lista));
            }
            // Devolvemos el dto
            return Ok(listaCategoriasDto);
        }

        // obtener 1 categoria
        [HttpGet("{categoriaId:int}", Name = "GetCategoria")] // Aceptamos el parametro 
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetCategoria(int categoriaId)
        {
            var itemCategoria = _ctRepo.GetCategoria(categoriaId);

            //Validamos si es null
            if (itemCategoria == null) { 
                return NotFound();
            }
            // Exponer el dto
            var itemCategoriaDto = _mapper.Map<CategoriaDto>(itemCategoria);
          
            // Devolvemos el dto
            return Ok(itemCategoriaDto);
        }

        // Crear una categoria
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult CrearCategoria([FromBody] CrearCategoriaDto crearCategoriaDto)
        {
            //Validamos si el modelo no es valido
            if (!ModelState.IsValid) { 
                return BadRequest(ModelState);
            }

            // Validamos si es null
            if(crearCategoriaDto == null) { return BadRequest(ModelState); }

            // Validamos si ya existe esa categoria
            if (_ctRepo.ExisteCategoria(crearCategoriaDto.Nombre))
            {
                ModelState.AddModelError("", "La categoria ya existe");
                return StatusCode(404, ModelState);
            }

            var categoria = _mapper.Map<Categoria>(crearCategoriaDto);

            // Validamos si no se pudo crear entonces devolvemos error
            if (!_ctRepo.CrearCategoria(categoria)) {
                ModelState.AddModelError("", $"Algo salio mal guardando el registro{categoria.Nombre}");
                return StatusCode(404, ModelState);
            }

            return CreatedAtRoute("GetCategoria", new {categoriaId = categoria.Id}, categoria);
        }

        // Actualizar una categoria parcialmente PATCH
        [HttpPatch("{categoriaId:int}", Name = "ActualizarPatchCategoria")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ActualizarPatchCategoria(int categoriaId, [FromBody] CategoriaDto categoriaDto)
        {
            //Validamos si el modelo no es valido
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validamos si es null o si no existe el id
            if (categoriaDto == null || categoriaId != categoriaDto.Id) { return BadRequest(ModelState); }
            var categoriaExistente = _ctRepo.GetCategoria(categoriaId);
            if (categoriaExistente == null) { return NotFound($"La categoria ingresada no existe{categoriaId}"); }

            var categoria = _mapper.Map<Categoria>(categoriaDto);

            // Validamos si no se pudo crear entonces devolvemos error
            if (!_ctRepo.ActualizarCategoria(categoria))
            {
                ModelState.AddModelError("", $"Algo salio mal actualizando el registro{categoria.Nombre}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        // Actualizar una categoria CON PUT
        [HttpPut("{categoriaId:int}", Name = "ActualizarPutCategoria")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ActualizarPutCategoria(int categoriaId, [FromBody] CategoriaDto categoriaDto)
        {
            //Validamos si el modelo no es valido
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validamos si es null o si no existe el id
            if (categoriaDto == null || categoriaId != categoriaDto.Id) { return BadRequest(ModelState); }
            var categoriaExistente = _ctRepo.GetCategoria(categoriaId);
            if (categoriaExistente == null) { return NotFound($"La categoria ingresada no existe{categoriaId}"); }

            var categoria = _mapper.Map<Categoria>(categoriaDto);

            // Validamos si no se pudo crear entonces devolvemos error
            if (!_ctRepo.ActualizarCategoria(categoria))
            {
                ModelState.AddModelError("", $"Algo salio mal actualizando el registro{categoria.Nombre}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        // Eliminar una categoria con DELETE
        [HttpDelete("{categoriaId:int}", Name = "BorrarCategoria")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult BorrarCategoria(int categoriaId)
        {
            // Validamos si existe la categoria
            if (!_ctRepo.ExisteCategoria(categoriaId)) { 
                return NotFound($"La categoria ingresada no existe{categoriaId}"); 
            }

            var categoria = _ctRepo.GetCategoria(categoriaId);

            // Validamos si no se pudo crear entonces devolvemos error
            if (!_ctRepo.BorrarCategoria(categoria))
            {
                ModelState.AddModelError("", $"Algo salio mal borrando el registro{categoria.Nombre}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}
