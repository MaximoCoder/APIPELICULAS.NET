using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Models.Dtos
{
    public class CrearPeliculaDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [MaxLength(100, ErrorMessage = "El numero maximo de caracteres es de 100")]
        public string Nombre { get; set; }
       
        public string Descripcion { get; set; }

        public int Duracion { get; set; }
        public string? RutaImagen { get; set; }
        public IFormFile Imagen {  get; set; }
        public enum CrearTipoClasificacion { Siete, Trece, Dieciseis, Dieciocho }
        public CrearTipoClasificacion Clasificacion { get; set; } // Este implementa el enum

        // Relacion con categoria
        public int categoriaId { get; set; }
    }
}
