using Lab06.Services;
using Lab06.Services;
using Lab06.Models;
using Lab06.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace Lab06.Controllers;

public class ArticlesController : Controller
{
    private readonly IArticleService _articleService;
    private readonly ICategoryService _categoryService;

    public ArticlesController(
        IArticleService articleService,
        ICategoryService categoryService)
    {
        _articleService = articleService;
        _categoryService = categoryService;
    }

    public async Task<IActionResult> Index(int page = 1, CancellationToken cancellationToken = default)
    {
        const int pageSize = 5;

        var totalItems = await _articleService.CountAsync(cancellationToken);
        var totalPages = totalItems == 0 ? 1 : (int)Math.Ceiling(totalItems / (double)pageSize);
        var currentPage = Math.Clamp(page, 1, totalPages);

        var articles = await _articleService.GetPagedAsync(currentPage, pageSize, cancellationToken);

        ViewBag.CurrentPage = currentPage;
        ViewBag.TotalPages = totalPages;

        return View(articles);
    }

    public async Task<IActionResult> Details(int? id, CancellationToken cancellationToken)
    {
        if (id is null)
        {
            return NotFound();
        }

        var article = await _articleService.GetByIdAsync(id.Value, cancellationToken);
        if (article is null)
        {
            return NotFound();
        }

        return View(article);
    }

    [Authorize]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var viewModel = new CreateArticleViewModel();
        await LoadDropdownsAsync(viewModel, cancellationToken);
        return View(viewModel);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateArticleViewModel viewModel, CancellationToken cancellationToken)
    {
        if (viewModel.Upload is null && Request.Form.Files.Count > 0)
        {
            viewModel.Upload = Request.Form.Files["Upload"] ?? Request.Form.Files[0];
        }

        if (!ModelState.IsValid)
        {
            await LoadDropdownsAsync(viewModel, cancellationToken);
            return View(viewModel);
        }

        var authorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        await _articleService.CreateAsync(viewModel, authorId, cancellationToken);
        return RedirectToAction(nameof(Index));
    }

    [Authorize]
    public async Task<IActionResult> Edit(int? id, CancellationToken cancellationToken)
    {
        if (id is null)
        {
            return NotFound();
        }

        var article = await _articleService.GetEntityByIdAsync(id.Value, cancellationToken);
        if (article is null)
        {
            return NotFound();
        }

        if (!IsOwnerOrAdmin(article))
        {
            return Forbid();
        }

        var viewModel = await _articleService.GetEditViewModelAsync(id.Value, cancellationToken);
        if (viewModel is null)
        {
            return NotFound();
        }

        await LoadDropdownsAsync(viewModel, cancellationToken);

        return View(viewModel);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EditArticleViewModel viewModel, CancellationToken cancellationToken)
    {
        if (id != viewModel.Id)
        {
            return NotFound();
        }

        var article = await _articleService.GetEntityByIdAsync(id, cancellationToken);
        if (article is null)
        {
            return NotFound();
        }

        if (!IsOwnerOrAdmin(article))
        {
            return Forbid();
        }

        if (viewModel.Upload is null && Request.Form.Files.Count > 0)
        {
            viewModel.Upload = Request.Form.Files["Upload"] ?? Request.Form.Files[0];
        }

        if (!ModelState.IsValid)
        {
            await LoadDropdownsAsync(viewModel, cancellationToken);
            return View(viewModel);
        }

        var updated = await _articleService.UpdateAsync(viewModel, cancellationToken);
        if (!updated)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [Authorize]
    public async Task<IActionResult> Delete(int? id, CancellationToken cancellationToken)
    {
        if (id is null)
        {
            return NotFound();
        }

        var article = await _articleService.GetByIdAsync(id.Value, cancellationToken);
        if (article is null)
        {
            return NotFound();
        }

        var articleEntity = await _articleService.GetEntityByIdAsync(id.Value, cancellationToken);
        if (articleEntity is null)
        {
            return NotFound();
        }

        if (!IsOwnerOrAdmin(articleEntity))
        {
            return Forbid();
        }

        return View(article);
    }

    [Authorize]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        var article = await _articleService.GetEntityByIdAsync(id, cancellationToken);
        if (article is null)
        {
            return NotFound();
        }

        if (!IsOwnerOrAdmin(article))
        {
            return Forbid();
        }

        await _articleService.DeleteAsync(id, cancellationToken);
        return RedirectToAction(nameof(Index));
    }

    private bool IsOwnerOrAdmin(Article article)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return article.AuthorId == userId || User.IsInRole("Admin");
    }

    private async Task LoadDropdownsAsync(CreateArticleViewModel viewModel, CancellationToken cancellationToken)
    {
        var categories = await _categoryService.GetAllAsync(cancellationToken);
        viewModel.Categories = categories
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            })
            .ToList();
    }
}
