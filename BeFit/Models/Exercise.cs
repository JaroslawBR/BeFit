using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeFit.Models
{
    public class Exercise : IValidatableObject
    {
        public int Id { get; set; }

        [Display(Name = "Obciążenie (kg)")]
        [Range(0, 1000, ErrorMessage = "Błędna wartość")]
        public double Weight { get; set; }

        [Display(Name = "Liczba serii")]
        [Range(1, 100, ErrorMessage = "Wartość musi być większa od 0")]
        public int Sets { get; set; }

        [Display(Name = "Liczba powtórzeń")]
        [Range(1, 1000, ErrorMessage = "Wartość musi być większa od 0")]
        public int Reps { get; set; }

        [Display(Name = "Typ ćwiczenia")]
        public int ExerciseTypeId { get; set; }

        [ForeignKey("ExerciseTypeId")]
        [Display(Name = "Typ ćwiczenia")]
        public virtual ExerciseType? ExerciseType { get; set; }

        [Display(Name = "Sesja treningowa")]
        public int TrainingSessionId { get; set; }

        [ForeignKey("TrainingSessionId")]
        [Display(Name = "Sesja treningowa")]
        public virtual TrainingSession? TrainingSession { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Sets * Reps <= 0)
            {
                yield return new ValidationResult(
                    "Trening musi zawierać przynajmniej jedną serię i jedno powtórzenie.",
                    new[] { nameof(Sets), nameof(Reps) });
            }
        }
    }
}