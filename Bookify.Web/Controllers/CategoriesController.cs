using Microsoft.AspNetCore.Mvc;
namespace Bookify.Web.Controllers
{
	public class CategoriesController : Controller
	{
		private readonly ApplicationDbContext _dbContext;

		public CategoriesController(ApplicationDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public IActionResult Index()
		{
			//TODO: 1.use ViewModel
			var categories = _dbContext.Categories.ToList();
			return View(categories);
		}
	}
}
