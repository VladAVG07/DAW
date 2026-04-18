using Lab06.Repositories;

namespace Lab06.Data;

public interface IUnitOfWork
{
    IArticleRepository Articles { get; }
    ICategoryRepository Categories { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
