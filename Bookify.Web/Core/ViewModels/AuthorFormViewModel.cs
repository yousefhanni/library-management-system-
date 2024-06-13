namespace Bookify.Web.Core.ViewModels
{
    public class AuthorFormViewModel
    {
        public int Id { get; set; } // Used for the Edit case

        [MaxLength(100, ErrorMessage = Errors.MaxLength), Display(Name = "Author")]
        [Remote("AllowItem", null!, AdditionalFields = "Id", ErrorMessage = Errors.Duplicated)]
        // The Remote attribute allows you to apply validation on the client side using code from the Controller
        public string Name { get; set; } = null!;
    }
}
