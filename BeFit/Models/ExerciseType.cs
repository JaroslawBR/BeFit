using System.ComponentModel.DataAnnotations;

namespace BeFit.Models
{
    public class ExerciseType
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nazwa jest wymagana")]
        [StringLength(100, ErrorMessage = "Nazwa zbyt długa")]
        [Display(Name = "Nazwa ćwiczenia")]
        public string Name { get; set; }
    }
}