using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ActivityCenter.Models
{    public class ActivityParticipant : BaseEntity
    {   
        public int ActivityParticipantID { get; set; }
        public int UserID { get; set; }
        public User User { get; set; }
        public int DojoActivityID {get; set;}
        public DojoActivity DojoActivity {get; set;}
        public DateTime Created_at { get; set; } = DateTime.Now;
        public DateTime Updated_at { get; set; } = DateTime.Now;
    }
}