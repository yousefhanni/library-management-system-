using Bookify.Web.Core.Models;
using Bookify.Web.Core.ViewModels;
using Bookify.Web.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            //TODO: use ViewModel 
            //AsNoTracking => To preserve performance
            var categories = _dbContext.Categories.AsNoTracking().ToList();
            return View(categories);
        }   

        [HttpGet]
        [AjaxOnly]
        public IActionResult Create()
        {
            return PartialView("_CreateEditForm");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateEditCategoryViewModel model)
        {
            if (!ModelState.IsValid)//Server side validation
                return BadRequest(); //

            var category = new Category { Name = model.Name };//Manual Mapping
            _dbContext.Add(category);
            _dbContext.SaveChanges();

            return PartialView("_CategoryRow", category);
        }

        [HttpGet]
        [AjaxOnly]
        public IActionResult Edit(int id)
        {
            var category = _dbContext.Categories.Find(id);
            if (category == null)
                return NotFound();

            var viewModel = new CreateEditCategoryViewModel //Manual Mapping
            {
                Id = id,
                Name = category.Name
            };
            return PartialView("_CreateEditForm", viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Edit(CreateEditCategoryViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var category = _dbContext.Categories.Find(model.Id);
            if (category == null)
                return NotFound();

            category.Name = model.Name;
            category.LastUpdatedOn = DateTime.Now;

            _dbContext.SaveChanges();

            return PartialView("_CategoryRow", category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleStatus(int id)
        {
            var category = _dbContext.Categories.Find(id);

            if (category is null)
                return NotFound();

            // Toggle the IsDeleted property of the category (switch between true and false)
            category.IsDeleted = !category.IsDeleted; //Switch
            category.LastUpdatedOn = DateTime.Now; 

            _dbContext.SaveChanges();
             
            return Ok(category.LastUpdatedOn.ToString());
        }
    }
}
