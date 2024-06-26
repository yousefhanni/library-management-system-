using Microsoft.AspNetCore.Mvc.Rendering;
using UoN.ExpressiveAnnotations.NetCore.Attributes;

namespace Bookify.Web.Core.ViewModels
{
    public class BookFormViewModel
    {
        public int Id { get; set; }

        [MaxLength(500, ErrorMessage = "The maximum length for the title is 500 characters.")]
        [Remote("AllowItem", null!, AdditionalFields = "Id,AuthorId", ErrorMessage = Errors.Duplicated)]

        public string Title { get; set; } = null!;

        // ID of the selected author.
        [Display(Name = "Author")]
        public int AuthorId { get; set; }

        // List of authors to populate the dropdown list.
        public IEnumerable<SelectListItem>? Authors { get; set; }

        // Publisher of the book with a maximum length of 200 characters.
        [MaxLength(200, ErrorMessage = "The maximum length for the publisher is 200 characters.")]
        public string Publisher { get; set; } = null!;

        // Publishing date of the book.
        [Display(Name = "Publishing Date")]
        [AssertThat("PublishingDate <= Today()", ErrorMessage = Errors.NotAllowFutureDates)]
        public DateTime PublishingDate { get; set; }=DateTime.Now;

        // Image file for the book.
        public IFormFile? Image { get; set; }

        public string? ImageUrl { get; set; }

        // Hall location of the book within the library or bookstore 
        [MaxLength(50, ErrorMessage = "The maximum length for the hall is 50 characters.")]
        public string Hall { get; set; } = null!;

        // Boolean flag indicating whether the book is available for rental.
        [Display(Name = "Is available for rental?")]
        public bool IsAvailableForRental { get; set; }

        public string Description { get; set; } = null!;

        //Stores a list of IDs of the selected categories.
        //IList => Readonly + has Index
        [Display(Name = "Categories")]
        public IList<int> SelectedCategories { get; set; } = new List<int>();

        // List of categories to populate the dropdown list.
        public IEnumerable<SelectListItem>? Categories { get; set; }
    }
}
