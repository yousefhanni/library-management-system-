using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bookify.Web.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public BooksController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        // Action method to return the Form view with an empty model
        public IActionResult Create()
        {
            return View("Form", PopulateViewModel());
        }

        // POST method to handle form submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BookFormViewModel model)
        {
            if (!ModelState.IsValid)
                // If not valid, repopulate the model with data and return the form view
                return View("Form", PopulateViewModel(model));
            
            //logic to save the book to the database 
              var book = _mapper.Map<Book>(model);

            // Add selected categories to the book
            foreach (var categorty in model.SelectedCategories)
                book.Categories.Add(new BookCategory { CategoryId = categorty });

             _context.Books.Add(book);
             _context.SaveChanges();
             
             //After saving, redirect to Index view
             return RedirectToAction(nameof(Index));

            return View("Form", PopulateViewModel(model)); // Temporary return statement for code completeness
        }

        // Method to populate the BookFormViewModel with data
        private BookFormViewModel PopulateViewModel(BookFormViewModel? model = null)
        {
            // Initialize the view model, if model is null, create a new instance
            BookFormViewModel viewModel = model is null ? new BookFormViewModel() : model;

            // Retrieve authors from the database, excluding those marked as deleted, and order by name
            var authors = _context.Authors.Where(a => !a.IsDeleted).OrderBy(a => a.Name).ToList();

            // Retrieve categories from the database, excluding those marked as deleted, and order by name
            var categories = _context.Categories.Where(a => !a.IsDeleted).OrderBy(a => a.Name).ToList();

            // Map the authors and categories to SelectListItem objects for use in dropdown lists
            viewModel.Authors = _mapper.Map<IEnumerable<SelectListItem>>(authors);
            viewModel.Categories = _mapper.Map<IEnumerable<SelectListItem>>(categories);

            // Return the populated view model
            return viewModel;
        }
    }
}
