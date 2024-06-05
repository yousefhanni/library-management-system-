using Microsoft.AspNetCore.Mvc;

namespace Bookify.Web.Core.ViewModels
{
    public class CreateEditCategoryViewModel
    {
        public int Id { get; set; } // Used for the Edit case

        [MaxLength(100, ErrorMessage = "Max length cannot exceed 100 characters")]
        [Remote("AllowItem", "Categories",AdditionalFields ="Id", ErrorMessage = "A category with the same name already exists!")]
        // The Remote attribute allows you to apply validation on the client side using code from the Controller
        public string Name { get; set; } = null!;
    }


}
