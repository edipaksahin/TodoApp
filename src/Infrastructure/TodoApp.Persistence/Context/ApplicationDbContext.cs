using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TodoApp.Application.Interface;
using TodoApp.Domain.Common;
using TodoApp.Domain.Entities;

namespace TodoApp.Persistence.Context
{
    public class ApplicationDbContext: DbContext, IApplicationDbContext
    {
        public DbSet<Domain.Entities.Task> Tasks { get; set; }
        public DbSet<User> Users { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker
                .Entries<BaseEntity>()
                .Where(i => i.State == EntityState.Added || i.State == EntityState.Modified || i.State == EntityState.Deleted))
            {
                if (entry.State == EntityState.Added)
                {
                    var id = entry.Property<Guid>(nameof(BaseEntity.Id)).CurrentValue;

                    if (id == Guid.Empty)
                    {
                        id = Guid.NewGuid();
                    }

                    entry.Property<DateTime>(nameof(BaseEntity.CreateDate)).CurrentValue = DateTime.UtcNow;
                }
                else
                {
                    if (entry.State == EntityState.Deleted)
                    {
                        entry.Property<bool>(nameof(BaseEntity.IsDeleted)).CurrentValue = true;
                        entry.State = EntityState.Modified;
                    }

                    entry.Property<DateTime?>(nameof(BaseEntity.UpdateDate)).CurrentValue = DateTime.UtcNow;

                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        public void DetachAllEntities()
        {
            var changedEntriesCopy = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added ||
                            e.State == EntityState.Modified ||
                            e.State == EntityState.Deleted)
                .ToList();

            foreach (var entry in changedEntriesCopy)
            {
                entry.State = EntityState.Detached;
            }
        }
    }
}
