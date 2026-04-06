using Lab06.Models;

namespace Lab06.Repositories;

public interface IArticleRepository : IRepository<Article>
{
    Task<List<Article>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default);
    Task<Article?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task<List<Article>> GetPagedWithDetailsAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<List<Article>> GetLatestWithDetailsAsync(int count, CancellationToken cancellationToken = default);
}
