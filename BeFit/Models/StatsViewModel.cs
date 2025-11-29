using System.ComponentModel.DataAnnotations;

namespace BeFit.Models
{
    public class StatsViewModel
    {
        [Display(Name = "Ćwiczenie")]
        public string ExerciseName { get; set; }

        [Display(Name = "Liczba treningów")]
        public int TimesPerformed { get; set; }

        [Display(Name = "Łączna suma powtórzeń")]
        public int TotalReps { get; set; }

        [Display(Name = "Średnie obciążenie (kg)")]
        [DisplayFormat(DataFormatString = "{0:N2}")] 
        public double AverageWeight { get; set; }

        [Display(Name = "Maksymalne obciążenie (kg)")]
        public double MaxWeight { get; set; }
    }
}