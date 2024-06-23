namespace Bookify.Web.Core.Models
{
    // Index attribute to ensure that the combination of Title and AuthorId is unique.
    [Index(nameof(Title), nameof(AuthorId), IsUnique = true)]
    public class Book : BaseModel
    {
        public int Id { get; set; }

        [MaxLength(500)]
        public string Title { get; set; } = null!;

        // Foreign key reference to the Author.
        public int AuthorId { get; set; }

        public Author? Author { get; set; } // Navigation property (one-to-one)

        [MaxLength(200)]
        public string Publisher { get; set; } = null!;

        public DateTime PublishingDate { get; set; }

        public string? ImageUrl { get; set; }

        // Location of the book within the library or bookstore 
        [MaxLength(50)]
        public string Hall { get; set; } = null!;

        // Indicates whether the book is available for rental.
        public bool IsAvailableForRental { get; set; }

        public string Description { get; set; } = null!;

        // Collection of categories associated with the book.
        public ICollection<BookCategory> Categories { get; set; } = new List<BookCategory>();
    }
}
