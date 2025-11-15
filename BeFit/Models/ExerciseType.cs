using System.ComponentModel.DataAnnotations;

namespace BeFit.Models
{
    public class ExerciseType
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)] // Ograniczenie długości nazwy do 100 znaków
        public string Name { get; set; }
    }
}
