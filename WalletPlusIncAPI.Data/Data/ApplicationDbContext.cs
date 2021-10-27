using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WalletPlusIncAPI.Models.Entities;

namespace WalletPlusIncAPI.Data.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {

        }

        public DbSet<Funding> Fundings { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
     
      

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
            modelBuilder.Entity<Transaction>()
                .HasOne(e => e.Wallet)
                .WithMany(e => e.Transactions)
                .HasForeignKey(e => e.WalletId)
                .OnDelete(DeleteBehavior.ClientCascade);

            modelBuilder.Entity<Wallet>()
                .HasOne(e => e.Owner)
                .WithMany(e => e.Wallets)
                .HasForeignKey(e => e.OwnerId)
                .OnDelete(DeleteBehavior.ClientCascade);

            //modelBuilder.Entity<Followers>()
            //    .HasNoKey();

            //modelBuilder.ApplyConfiguration(new AppUserConfig());
            //modelBuilder.ApplyConfiguration(new RoleConfig());


        }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)

        {
            var entries = ChangeTracker
                .Entries()
                .Where(entry => entry.Entity is BaseEntity && (
                    entry.State == EntityState.Added
                    || entry.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                ((BaseEntity)entityEntry.Entity).Updated_at = DateTime.Now;

                if (entityEntry.State == EntityState.Added)
                {
                    ((BaseEntity)entityEntry.Entity).Created_at = DateTime.Now;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
