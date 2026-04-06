using Lab06.ViewModels;

namespace Lab06.Services;

public interface IArticleService
{
    Task<List<ArticleViewModel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<ArticleViewModel>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task<List<ArticleViewModel>> GetLatestAsync(int count, CancellationToken cancellationToken = default);
    Task<ArticleViewModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task CreateAsync(CreateArticleViewModel viewModel, CancellationToken cancellationToken = default);
    Task<EditArticleViewModel?> GetEditViewModelAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(EditArticleViewModel viewModel, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
