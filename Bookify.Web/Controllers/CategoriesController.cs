namespace Bookify.Web.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
       private readonly IMapper _mapper;
        public CategoriesController(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            var categories = _dbContext.Categories.AsNoTracking().ToList();

            var viewModel = _mapper.Map<IEnumerable<CategoryViewModel>>(categories);

            return View(viewModel);
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

            var category = _mapper.Map<Category>(model);
            _dbContext.Add(category);
            _dbContext.SaveChanges();

            var viewModel = _mapper.Map<CategoryViewModel>(category);

            return PartialView("_CategoryRow", viewModel);
        }

        [HttpGet]
        [AjaxOnly]
        public IActionResult Edit(int id)
        {
            var category = _dbContext.Categories.Find(id);
            if (category == null)
                return NotFound();

            var viewModel = _mapper.Map<CreateEditCategoryViewModel>(category); 
          
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

            category = _mapper.Map(model, category);
            category.LastUpdatedOn = DateTime.Now;

            _dbContext.SaveChanges();

            var viewModel = _mapper.Map<CategoryViewModel>(category);

            return PartialView("_CategoryRow", viewModel);
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


        public IActionResult AllowItem(CreateEditCategoryViewModel model)
        {
            // Retrieve the category from the database that matches the name provided in the model
            var category = _dbContext.Categories.SingleOrDefault(c => c.Name == model.Name);

            // Check if the category does not exist or if the existing category's ID matches the model's ID
            var isAllowed = category == null || category.Id.Equals(model.Id);

            // Return a JSON response indicating whether the category name is allowed (i.e., it doesn't exist or it matches the current category)
            return Json(isAllowed);
        }

    }
}
