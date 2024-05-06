﻿using Bookify.Web.Core.Models;
using Bookify.Web.Core.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
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
        public IActionResult Create()
        {
            return View("CreateEdit");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateEditCategoryViewModel model)
        {
            if (!ModelState.IsValid)//Server side validation
                return View(model);

            var category = new Category { Name = model.Name };//Manual Mapping
            _dbContext.Add(category);
            _dbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
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
            return View("CreateEdit", viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Edit(CreateEditCategoryViewModel model)
        {
            if (!ModelState.IsValid)//Server side validation
                return View("CreateEdit", model);

            var category = _dbContext.Categories.Find(model.Id);
            if (category == null)
                return NotFound();

            category.Name = model.Name;
            category.LastUpdatedOn = DateTime.Now;

            _dbContext.Update(category);
            _dbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
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
