using System.ComponentModel.DataAnnotations;

namespace BeFit.Models
{
    public class TrainingSession
    {
        public int Id { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        // TODO: Opcjonalna walidacja sprawdzająca, czy
        // EndTime jest późniejsze niż StartTime
    }
}