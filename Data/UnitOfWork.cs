using Lab06.Repositories;

namespace Lab06.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(
        AppDbContext context,
        IArticleRepository articleRepository,
        ICategoryRepository categoryRepository)
    {
        _context = context;
        Articles = articleRepository;
        Categories = categoryRepository;
    }

    public IArticleRepository Articles { get; }
    public ICategoryRepository Categories { get; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
