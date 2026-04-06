using Lab06.Repositories;
using Lab06.Models;

namespace Lab06.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(
        AppDbContext context,
        IArticleRepository articleRepository,
        ICategoryRepository categoryRepository,
        IRepository<User> userRepository)
    {
        _context = context;
        Articles = articleRepository;
        Categories = categoryRepository;
        Users = userRepository;
    }

    public IArticleRepository Articles { get; }
    public ICategoryRepository Categories { get; }
    public IRepository<User> Users { get; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
