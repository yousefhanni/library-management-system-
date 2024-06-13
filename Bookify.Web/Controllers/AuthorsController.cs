namespace Bookify.Web.Controllers
{
    public class AuthorsController : Controller
    {

        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        public AuthorsController(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            var Authors = _dbContext.Authors.AsNoTracking().ToList();

            var viewModel = _mapper.Map<IEnumerable<AuthorViewModel>>(Authors);

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
        public IActionResult Create(AuthorFormViewModel model)
        {
            if (!ModelState.IsValid)//Server side validation
                return BadRequest(); //

            var Author = _mapper.Map<Author>(model);
            _dbContext.Add(Author);
            _dbContext.SaveChanges();

            var viewModel = _mapper.Map<AuthorViewModel>(Author);

            return PartialView("_AuthorRow", viewModel);
        }

        [HttpGet]
        [AjaxOnly]
        public IActionResult Edit(int id)
        {
            var Author = _dbContext.Authors.Find(id);
            if (Author == null)
                return NotFound();

            var viewModel = _mapper.Map<AuthorFormViewModel>(Author);

            return PartialView("_CreateEditForm", viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Edit(AuthorFormViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var Author = _dbContext.Authors.Find(model.Id);
            if (Author == null)
                return NotFound();

            Author = _mapper.Map(model, Author);
            Author.LastUpdatedOn = DateTime.Now;

            _dbContext.SaveChanges();

            var viewModel = _mapper.Map<AuthorViewModel>(Author);

            return PartialView("_AuthorRow", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleStatus(int id)
        {
            var Author = _dbContext.Authors.Find(id);

            if (Author is null)
                return NotFound();

            // Toggle the IsDeleted property of the Author (switch between true and false)
            Author.IsDeleted = !Author.IsDeleted; //Switch
            Author.LastUpdatedOn = DateTime.Now;

            _dbContext.SaveChanges();

            return Ok(Author.LastUpdatedOn.ToString());
        }


        public IActionResult AllowItem(AuthorFormViewModel model)
        {
            // Retrieve the Author from the database that matches the name provided in the model
            var Author = _dbContext.Authors.SingleOrDefault(c => c.Name == model.Name);

            // Check if the Author does not exist or if the existing Author's ID matches the model's ID
            var isAllowed = Author == null || Author.Id.Equals(model.Id);

            // Return a JSON response indicating whether the Author name is allowed (i.e., it doesn't exist or it matches the current Author)
            return Json(isAllowed);
        }

    }
}
