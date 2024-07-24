using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace Bookify.Web.Controllers
{
    public class BooksController : Controller
    {
        // This is useful for accessing the web root path(wwwroot) where the images will be saved.
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        //private field To access on Cloudinary service 
        private readonly Cloudinary _cloudinary;

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
        //(IOptions)=>facilitates binding configuration values from appsettings.json to class like CloudinarySettings 
        public BooksController(ApplicationDbContext context, IMapper mapper,
                  IWebHostEnvironment webHostEnvironment, IOptions<CloudinarySettings> cloudinary)
        {
            _context = context;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;

            // Initialize Cloudinary account using values from CloudinarySettings
            Account account = new()
            {
                Cloud = cloudinary.Value.Cloud,
                ApiKey = cloudinary.Value.ApiKey,
                ApiSecret = cloudinary.Value.ApiSecret
            };

            _cloudinary = new Cloudinary(account);
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
        public async Task<IActionResult> Create(BookFormViewModel model)
        {
            if (!ModelState.IsValid)
                // If not valid, repopulate the model with data and return the form view
                return View("Form", PopulateViewModel(model));

            var book = _mapper.Map<Book>(model);

            // Check if an image is uploaded
            if (model.Image != null)
            {
                var extension = Path.GetExtension(model.Image.FileName); // Get the file extension of the uploaded image

                if (!_allowedExtensions.Contains(extension)) // Check if the extension is allowed
                {
                    ModelState.AddModelError(nameof(model.Image), "Not allowed extension"); // Add model error for disallowed extension
                    return View("Form", PopulateViewModel(model)); // Return to the form view with populated view model
                }

                if (model.Image.Length > _maxAllowedSize) // Check if the image size exceeds the maximum allowed size
                {
                    ModelState.AddModelError(nameof(model.Image), "Maximum size exceeded");
                    return View("Form", PopulateViewModel(model));
                }

                var imageName = $"{Guid.NewGuid()}{extension}"; // Generate a unique image name using GUID and extension

                //Save image on Server 
                //var path = Path.Combine($"{_webHostEnvironment.WebRootPath}/images/books", imageName);

                //using var stream = System.IO.File.Create(path);
                //await model.Image.CopyToAsync(stream);
                //book.ImageUrl = imageName;


                //To Save image on Cloudinary => 

                // Open the image file stream for reading
                using var stream = model.Image.OpenReadStream();

                // Define parameters for the image upload, including file description and usage of the filename
                var imageParams = new ImageUploadParams()
                {
                    File = new FileDescription(imageName, stream),

                    // Specify that the uploaded file should use the same name (GUID) as provided
                    UseFilename = true
                };

                var result = await _cloudinary.UploadAsync(imageParams);

                book.ImageUrl = result.SecureUrl.ToString();  // receive Url from cloudinary then Add to ImageUrl of Book
                book.ImageThumbnailUrl = GetThumbnailUrl(book.ImageUrl);   
                book.ImagePublicId = result.PublicId;
                
            }

            // Add selected categories to the book
            foreach (var categoryId in model.SelectedCategories)
                book.Categories.Add(new BookCategory { CategoryId = categoryId });

            _context.Books.Add(book);
            await _context.SaveChangesAsync(); // Save changes asynchronously

            // After saving, redirect to Index view
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // Retrieve the book from the database, including its categories
            var book = await _context.Books.Include(b => b.Categories).SingleOrDefaultAsync(b => b.Id == id);

            if (book == null)
                return NotFound();

            var model = _mapper.Map<BookFormViewModel>(book);
            var viewModel = PopulateViewModel(model);

            // Set the selected categories in the view model based on the book's categories
            viewModel.SelectedCategories = book.Categories.Select(c => c.CategoryId).ToList();

            return View("Form", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BookFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Form", PopulateViewModel(model));

            // Retrieve the book from the database, including its categories
            var book = await _context.Books.Include(b => b.Categories).SingleOrDefaultAsync(b => b.Id == model.Id);

            if (book == null)
                return NotFound();
            string imagePublicId = null;
            // Check if a new image is uploaded
            if (model.Image != null)
            {
                // If the book already has an image, delete the old image file
                if (!string.IsNullOrEmpty(book.ImageUrl))
                {
                    //Apply delete on server
                    //var oldImagePath = Path.Combine($"{_webHostEnvironment.WebRootPath}/images/books", book.ImageUrl);

                    //if (System.IO.File.Exists(oldImagePath))
                    //    System.IO.File.Delete(oldImagePath);

                    //Apply delete on Cloudinary
                    await _cloudinary.DeleteResourcesAsync(book.ImagePublicId);

                }

                // Validate the image file extension
                var extension = Path.GetExtension(model.Image.FileName);

                if (!_allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError(nameof(model.Image), "Not allowed extension");
                    return View("Form", PopulateViewModel(model));
                }

                // Validate the image file size
                if (model.Image.Length > _maxAllowedSize)
                {
                    ModelState.AddModelError(nameof(model.Image), "Maximum size exceeded");
                    return View("Form", PopulateViewModel(model));
                }

                var imageName = $"{Guid.NewGuid()}{extension}";


                //Edit image on Server 
                //var path = Path.Combine($"{_webHostEnvironment.WebRootPath}/images/books", imageName);

                //using var stream = System.IO.File.Create(path);
                //await model.Image.CopyToAsync(stream);

                //model.ImageUrl = imageName;


                //Edit image on Cloudinary
                using var straem = model.Image.OpenReadStream();

                var imageParams = new ImageUploadParams
                {
                    File = new FileDescription(imageName, straem),
                    UseFilename = true
                };

                var result = await _cloudinary.UploadAsync(imageParams);

                model.ImageUrl = result.SecureUrl.ToString();
                imagePublicId = result.PublicId;
            }
            // If no new image is uploaded and the book already has an image, retain the old image URL
            else if (!string.IsNullOrEmpty(book.ImageUrl))
                model.ImageUrl = book.ImageUrl;

            book = _mapper.Map(model, book);
            book.LastUpdatedOn = DateTime.Now;
            book.ImageThumbnailUrl = GetThumbnailUrl(book.ImageUrl!);
            book.ImagePublicId = imagePublicId;
                
            foreach (var categoryId in model.SelectedCategories)
                book.Categories.Add(new BookCategory { CategoryId = categoryId });

            // Save the changes to the database asynchronously
            await _context.SaveChangesAsync();

            // Redirect to the Index action
            return RedirectToAction(nameof(Index));
        }

        // Method to populate the BookFormViewModel with data
        private BookFormViewModel PopulateViewModel(BookFormViewModel? model = null)
        {
            // Initialize the view model, if model is null, create a new instance
            BookFormViewModel viewModel = model ?? new BookFormViewModel();

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

        // This endpoint checks if a book title is unique for a given author,
        // allowing the creation or updating of a book record without duplication.
        public IActionResult AllowItem(BookFormViewModel model)
        {
            // Retrieve a book from the database that has the same title and author ID as the model
            var book = _context.Books.SingleOrDefault(b => b.Title == model.Title && b.AuthorId == model.AuthorId);

            // Determine if the book is allowed:
            var isAllowed = book == null || book.Id == model.Id;

            // Return the result as a JSON response
            return Json(isAllowed);
        }
        private string GetThumbnailUrl(string url)
        {
            var separator = "image/upload/";
            var urlParts = url.Split(separator);

            var thumbnailUrl = $"{urlParts[0]}{separator}c_thumb,w_200,g_face/{urlParts[1]}";

            return thumbnailUrl;
        }
    }
}
