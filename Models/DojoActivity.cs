using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ActivityCenter.Models
{    public class DojoActivity : BaseEntity
    {   
        public int DojoActivityID { get; set; }
        public string Title { get; set; }
        public TimeSpan ActivityTime { get; set; }
        public DateTime ActivityDate { get; set; }
        public int Duration { get; set; }
        public string Description { get; set; }
        public string HourOrMin {get; set;}
        public int CreatorID {get; set;}
        public User Creator {get; set;}
        public List<ActivityParticipant> Participants {get; set;} = new List<ActivityParticipant>();
        public DateTime Created_at { get; set; } = DateTime.Now;
        public DateTime Updated_at { get; set; } = DateTime.Now;

        public string ActivityDateTime{
            get{
                return ActivityDate.ToString("MM/dd") + "@" + ActivityTime.ToString("tt");
            }
        }
    }
}