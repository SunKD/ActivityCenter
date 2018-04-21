using System.ComponentModel.DataAnnotations;
using System;
using ActivityContext.Models;

namespace ActivityCenter.Models
{
    public class ActivityViewModel : BaseEntity
    {
        [Display(Name = "Title:")]
        [Required]
        [MinLength(2)]
        public string Title { get; set; }

        [Display(Name = "Description:")]
        [Required]
        [MinLength(10)]
        public string Description { get; set; }

        [Display(Name = "Time:")]
        [Required]
        [DataType(DataType.Time)]
        public TimeSpan ActivityTime { get; set; }

        [Display(Name = "Date:")]
        [Required]
        [CurrentDate(ErrorMessage = "Date must be after or equal to current date")]
        public DateTime ActivityDate { get; set; }

        [Display(Name = "Duration:")]
        [Required]
        public int Duration { get; set; }

    }
}