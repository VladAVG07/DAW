using Lab06.Data;
using Lab06.Models;
using Lab06.ViewModels;
using Lab06.Data;

namespace Lab06.Services;

public class ArticleService : IArticleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _environment;

    public ArticleService(IUnitOfWork unitOfWork, IWebHostEnvironment environment)
    {
        _unitOfWork = unitOfWork;
        _environment = environment;
    }

    public async Task<List<ArticleViewModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var articles = await _unitOfWork.Articles.GetAllWithDetailsAsync(cancellationToken);

        return articles.Select(MapToArticleViewModel).ToList();
    }

    public async Task<List<ArticleViewModel>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var safePage = Math.Max(1, page);
        var safePageSize = Math.Max(1, pageSize);

        var articles = await _unitOfWork.Articles.GetPagedWithDetailsAsync(safePage, safePageSize, cancellationToken);
        return articles.Select(MapToArticleViewModel).ToList();
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Articles.CountAsync(cancellationToken);
    }

    public async Task<List<ArticleViewModel>> GetLatestAsync(int count, CancellationToken cancellationToken = default)
    {
        var safeCount = Math.Max(1, count);
        var articles = await _unitOfWork.Articles.GetLatestWithDetailsAsync(safeCount, cancellationToken);
        return articles.Select(MapToArticleViewModel).ToList();
    }

    public async Task<ArticleViewModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var article = await _unitOfWork.Articles.GetByIdWithDetailsAsync(id, cancellationToken);
        return article is null ? null : MapToArticleViewModel(article);
    }

    public async Task<Article?> GetEntityByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Articles.GetByIdAsync(id, cancellationToken);
    }

    public async Task CreateAsync(CreateArticleViewModel viewModel, string? authorId, CancellationToken cancellationToken = default)
    {
        var article = new Article
        {
            Title = viewModel.Title,
            Content = viewModel.Content,
            CategoryId = viewModel.CategoryId,
            AuthorId = authorId,
            PublishedAt = DateTime.Now,
            ImagePath = await SaveImageAsync(viewModel.Upload, cancellationToken)
        };

        await _unitOfWork.Articles.AddAsync(article, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<EditArticleViewModel?> GetEditViewModelAsync(int id, CancellationToken cancellationToken = default)
    {
        var article = await _unitOfWork.Articles.GetByIdAsync(id, cancellationToken);
        if (article is null)
        {
            return null;
        }

        var viewModel = new EditArticleViewModel
        {
            Id = article.Id,
            Title = article.Title,
            Content = article.Content,
            CategoryId = article.CategoryId,
            ExistingImagePath = article.ImagePath
        };

        return viewModel;
    }

    public async Task<bool> UpdateAsync(EditArticleViewModel viewModel, CancellationToken cancellationToken = default)
    {
        var article = await _unitOfWork.Articles.GetByIdAsync(viewModel.Id, cancellationToken);
        if (article is null)
        {
            return false;
        }

        article.Title = viewModel.Title;
        article.Content = viewModel.Content;
        article.CategoryId = viewModel.CategoryId;

        var newImagePath = await SaveImageAsync(viewModel.Upload, cancellationToken);
        article.ImagePath = newImagePath ?? viewModel.ExistingImagePath;

        _unitOfWork.Articles.Update(article);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var article = await _unitOfWork.Articles.GetByIdAsync(id, cancellationToken);
        if (article is null)
        {
            return false;
        }

        _unitOfWork.Articles.Remove(article);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task<string?> SaveImageAsync(IFormFile? upload, CancellationToken cancellationToken)
    {
        if (upload is null || upload.Length == 0)
        {
            return null;
        }

        var originalFileName = Path.GetFileName(upload.FileName);
        var extension = Path.GetExtension(originalFileName);
        var fileName = $"{Guid.NewGuid():N}{extension}";
        var imagesPath = Path.Combine(_environment.WebRootPath, "images");
        Directory.CreateDirectory(imagesPath);

        var savePath = Path.Combine(imagesPath, fileName);
        await using var stream = File.Create(savePath);
        await upload.CopyToAsync(stream, cancellationToken);

        return $"/images/{fileName}";
    }

    private static ArticleViewModel MapToArticleViewModel(Article article)
    {
        return new ArticleViewModel
        {
            Id = article.Id,
            Title = article.Title,
            Content = article.Content,
            PublishedAt = article.PublishedAt,
            CategoryName = article.Category?.Name ?? "N/A",
            AuthorName = article.Author?.FullName ?? "N/A",
            ImagePath = article.ImagePath
        };
    }
}
