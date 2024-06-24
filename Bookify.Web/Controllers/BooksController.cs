using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bookify.Web.Controllers
{
    public class BooksController : Controller
    {
        private  readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        /// Rule: When dealing with any file inside the application, ensure two things:
        /// 1. Allowed Extensions: 
        ///    - Only accept specified extensions.
        ///    - Not accepting any type of extension prevents hacking risks.
        ///    - Example: A hacker might send a script or an executable file to hack the system.
        /// 2. File Size:
        ///    - Ensure the file size is within acceptable limits.
        ///    - This prevents excessive storage usage and potential attacks.

        private List<string> _allowedExtensions = new() { ".jpg", ".jpeg", ".png" };
        private int _maxAllowedSize = 2097152;

        public BooksController(ApplicationDbContext context, IMapper mapper,
            IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        // Action method to return the Form view with an empty model
        [HttpGet]
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
            
              var book = _mapper.Map<Book>(model);

            // Check if an image is uploaded
            if (model.Image is not null) 
            {
                var extension = Path.GetExtension(model.Image.FileName); // Get the file extension of the uploaded image

                if (!_allowedExtensions.Contains(extension)) // Check if the extension is allowed
                {
                    ModelState.AddModelError(nameof(model.Image), Errors.NotAllowedExtension); // Add model error for disallowed extension
                    return View("Form", PopulateViewModel(model)); // Return to the form view with populated view model
                }

                if (model.Image.Length > _maxAllowedSize) // Check if the image size exceeds the maximum allowed size
                {
                    ModelState.AddModelError(nameof(model.Image), Errors.MaxSize);
                    return View("Form", PopulateViewModel(model)); 
                }

                var imageName = $"{Guid.NewGuid()}{extension}"; // Generate a unique image name using GUID and extension

                var path = Path.Combine($"{_webHostEnvironment.WebRootPath}/images/books", imageName); // Combine the web root path with the image directory and name

                using var stream = System.IO.File.Create(path); // Create a file stream 
                model.Image.CopyTo(stream); // Copy the uploaded image data to the file stream

                book.ImageUrl = imageName; // Assign the image URL to the book object
            }

            // Add selected categories to the book
            foreach (var categorty in model.SelectedCategories)
                book.Categories.Add(new BookCategory { CategoryId = categorty });

             _context.Books.Add(book);
             _context.SaveChanges();
             
             //After saving, redirect to Index view
             return RedirectToAction(nameof(Index));

            return View("Form", PopulateViewModel(model)); // Temporary return statement for code completeness
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            // Retrieve the book from the database, including its categories
            var book = _context.Books.Include(b => b.Categories).SingleOrDefault(b => b.Id == id);

            if (book is null)
                return NotFound();

            var model = _mapper.Map<BookFormViewModel>(book);
            var viewModel = PopulateViewModel(model);

            // Set the selected categories in the view model based on the book's categories
            viewModel.SelectedCategories = book.Categories.Select(c => c.CategoryId).ToList();

            return View("Form", viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(BookFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Form", PopulateViewModel(model));

            // Retrieve the book from the database, including its categories
            var book = _context.Books.Include(b => b.Categories).SingleOrDefault(b => b.Id == model.Id);

            if (book is null)
                return NotFound();

            // Check if a new image is uploaded
            if (model.Image is not null)
            {
                // If the book already has an image, delete the old image file
                if (!string.IsNullOrEmpty(book.ImageUrl))
                {
                    var oldImagePath = Path.Combine($"{_webHostEnvironment.WebRootPath}/images/books", book.ImageUrl);

                    if (System.IO.File.Exists(oldImagePath))
                        System.IO.File.Delete(oldImagePath);
                }

                // Validate the image file extension
                var extension = Path.GetExtension(model.Image.FileName);

                if (!_allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError(nameof(model.Image), Errors.NotAllowedExtension);
                    return View("Form", PopulateViewModel(model));
                }

                // Validate the image file size
                if (model.Image.Length > _maxAllowedSize)
                {
                    ModelState.AddModelError(nameof(model.Image), Errors.MaxSize);
                    return View("Form", PopulateViewModel(model));
                }

                // Generate a new unique name for the image file
                var imageName = $"{Guid.NewGuid()}{extension}";

                // Save the new image file to the server
                var path = Path.Combine($"{_webHostEnvironment.WebRootPath}/images/books", imageName);

                using var stream = System.IO.File.Create(path);
                model.Image.CopyTo(stream);

                // Update the model's ImageUrl property with the new image name
                model.ImageUrl = imageName;
            }
            // If no new image is uploaded and the book already has an image, retain the old image URL
            else if (model.Image is null && !string.IsNullOrEmpty(book.ImageUrl))
                model.ImageUrl = book.ImageUrl;

            // Map the updated model properties to the existing book entity
            book = _mapper.Map(model, book);
            book.LastUpdatedOn = DateTime.Now;

            // Update the book's categories with the selected categories
            foreach (var category in model.SelectedCategories)
                book.Categories.Add(new BookCategory { CategoryId = category });

            // Save the changes to the database
            _context.SaveChanges();

            // Redirect to the Index action
            return RedirectToAction(nameof(Index));
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
