using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Interface
{
    public interface IApplicationDbContext
    {
        DbSet<Domain.Entities.Task> Tasks { get; set; }
        DbSet<User> Users { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        void DetachAllEntities();
    }
}
