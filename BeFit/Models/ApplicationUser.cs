using Microsoft.AspNetCore.Identity;

namespace BeFit.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<TrainingSession> TrainingSessions { get; set; }
    }
}