using Domain.Entities; // Antager du har en mappe til dette i Domain-projektet
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // Her tilføjer du dine tabeller, f.eks.:
        public DbSet<VolunteerOffer> VolunteerOffers { get; set; }
    }
}