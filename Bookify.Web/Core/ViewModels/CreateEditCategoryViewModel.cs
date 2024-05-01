namespace Bookify.Web.Core.ViewModels
{
    public class CreateEditCategoryViewModel
    {
        public int Id { get; set; } //to case Edit

        [MaxLength(100,ErrorMessage = "Max length cannot be more than 5 characters")]
        public string Name { get; set; } = null!;
    }
}
