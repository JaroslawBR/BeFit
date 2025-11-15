using System.ComponentModel.DataAnnotations.Schema;

namespace BeFit.Models
{
    public class Exercise
    {
        public int Id { get; set; }

        public double Weight { get; set; } // Obciążenie
        public int Sets { get; set; }      // Liczba serii
        public int Reps { get; set; }      // Liczba powtórzeń w serii

        // Klucz obcy do ExerciseType
        public int ExerciseTypeId { get; set; }
        [ForeignKey("ExerciseTypeId")]
        public virtual ExerciseType ExerciseType { get; set; }

        // Klucz obcy do TrainingSession
        public int TrainingSessionId { get; set; }
        [ForeignKey("TrainingSessionId")]
        public virtual TrainingSession TrainingSession { get; set; }

        // W przyszłości można dodać powiązanie z użytkownikiem
        // public string AppUserId { get; set; }
        // [ForeignKey("AppUserId")]
        // public virtual AppUser AppUser { get; set; }
    }
}