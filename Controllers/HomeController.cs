using System.Diagnostics;
using System.Diagnostics;
using Lab06.Models;
using Lab06.Services;
using Lab06.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Lab06.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IArticleService _articleService;
        private readonly ICategoryService _categoryService;

        public HomeController(
            ILogger<HomeController> logger,
            IArticleService articleService,
            ICategoryService categoryService)
        {
            _logger = logger;
            _articleService = articleService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var viewModel = new HomeViewModel
            {
                LatestArticles = await _articleService.GetLatestAsync(3, cancellationToken),
                TotalArticles = await _articleService.CountAsync(cancellationToken),
                TotalCategories = await _categoryService.CountAsync(cancellationToken)
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
