namespace Bookify.Web.Core.Mapping
{
    // This class defines the mapping configuration for application-specific objects,
    // mapping properties from one type to another.
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //Category
            CreateMap<Category, CategoryViewModel>();
            CreateMap<CategoryFormViewModel, Category>().ReverseMap();

            //Author
            CreateMap<Author, AuthorViewModel>();
            CreateMap<AuthorFormViewModel, Author>().ReverseMap();
        }

    }

}
