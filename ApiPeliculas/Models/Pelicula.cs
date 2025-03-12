using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiPeliculas.Models
{
    public class Pelicula
    {
        [Key]
        public int Id { get; set; }
        
        public string Nombre { get; set; }
        
        public string Descripcion { get; set; }
       
        public int Duracion { get; set; }
        public string RutaImagen {  get; set; }
        
        public enum TipoClasificacion { Siete, Trece, Dieciseis, Dieciocho }
        public TipoClasificacion Clasificacion {  get; set; } // Este implementa el enum
        
        public DateTime FechaCreacion { get; set; }

        // Relacion con categoria
        public int categoriaId { get; set; }
        [ForeignKey("categoriaId")]
        public Categoria Categoria { get; private set; }
    }
}
