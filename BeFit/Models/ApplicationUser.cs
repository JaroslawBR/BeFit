using Microsoft.AspNetCore.Identity;

namespace BeFit.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Relacja do sesji treningowych
        public virtual ICollection<TrainingSession> TrainingSessions { get; set; }
    }
}