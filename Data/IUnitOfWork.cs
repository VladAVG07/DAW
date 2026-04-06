using Lab06.Repositories;
using Lab06.Models;

namespace Lab06.Data;

public interface IUnitOfWork
{
    IArticleRepository Articles { get; }
    ICategoryRepository Categories { get; }
    IRepository<User> Users { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
