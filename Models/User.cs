using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ActivityCenter.Models
{    public class User : BaseEntity
    {   
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        [InverseProperty("Creator")]
        public List<DojoActivity> CreatedActivities {get; set;} = new List<DojoActivity>();
        // // [InverseProperty("ActivityID")]
        public List<DojoActivity> JoinedActivities  {get; set;} = new List<DojoActivity>();
        public DateTime Created_at { get; set; } = DateTime.Now;
        public DateTime Updated_at { get; set; } = DateTime.Now;
    }
}