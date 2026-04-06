using Lab06.Services;
using Lab06.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lab06.Controllers;

public class ArticlesController : Controller
{
    private readonly IArticleService _articleService;
    private readonly ICategoryService _categoryService;
    private readonly IUserService _userService;

    public ArticlesController(
        IArticleService articleService,
        ICategoryService categoryService,
        IUserService userService)
    {
        _articleService = articleService;
        _categoryService = categoryService;
        _userService = userService;
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

    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var viewModel = new CreateArticleViewModel();
        await LoadDropdownsAsync(viewModel, cancellationToken);
        return View(viewModel);
    }

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

        await _articleService.CreateAsync(viewModel, cancellationToken);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id, CancellationToken cancellationToken)
    {
        if (id is null)
        {
            return NotFound();
        }

        var viewModel = await _articleService.GetEditViewModelAsync(id.Value, cancellationToken);
        if (viewModel is null)
        {
            return NotFound();
        }

        await LoadDropdownsAsync(viewModel, cancellationToken);

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EditArticleViewModel viewModel, CancellationToken cancellationToken)
    {
        if (id != viewModel.Id)
        {
            return NotFound();
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

        return View(article);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        await _articleService.DeleteAsync(id, cancellationToken);
        return RedirectToAction(nameof(Index));
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

        var users = await _userService.GetAllAsync(cancellationToken);
        viewModel.Users = users
            .Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = u.Name
            })
            .ToList();
    }
}
