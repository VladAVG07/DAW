using Lab06.Data;
using Lab06.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

namespace Lab06.Repositories;

public class ArticleRepository : Repository<Article>, IArticleRepository
{
    public ArticleRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<Article>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Articles
            .Include(a => a.Category)
            .Include(a => a.User)
            .OrderByDescending(a => a.PublishedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Article?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await Context.Articles
            .Include(a => a.Category)
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Articles.CountAsync(cancellationToken);
    }

    public async Task<List<Article>> GetPagedWithDetailsAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await Context.Articles
            .Include(a => a.Category)
            .Include(a => a.User)
            .OrderByDescending(a => a.PublishedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Article>> GetLatestWithDetailsAsync(int count, CancellationToken cancellationToken = default)
    {
        return await Context.Articles
            .Include(a => a.Category)
            .Include(a => a.User)
            .OrderByDescending(a => a.PublishedAt)
            .Take(count)
            .ToListAsync(cancellationToken);
    }
}
