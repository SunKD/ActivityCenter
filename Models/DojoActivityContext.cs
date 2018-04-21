using Microsoft.EntityFrameworkCore;

namespace ActivityCenter.Models
{
    public class DojoActivityContext : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter along
        public DojoActivityContext(DbContextOptions<DojoActivityContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<DojoActivity> Activities {get; set;}
        public DbSet<ActivityParticipant> ActivityParticipants {get;set;}
    }
}