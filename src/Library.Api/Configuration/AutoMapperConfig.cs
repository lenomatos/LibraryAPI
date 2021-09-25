using AutoMapper;
using Library.Api.ViewModels;
using Library.Business.Models;

namespace Library.Api.Configuration
{
    public class AutoMapperConfig: Profile
    {
        public AutoMapperConfig()
        {

            CreateMap<Author, AuthorViewModel>().ReverseMap();
            CreateMap<Book, BookViewModel>().ReverseMap();
            CreateMap<BookGenre, BookGenreViewModel>().ReverseMap();
            CreateMap<Reserve, ReserveViewModel>().ReverseMap();
            CreateMap<User, UserViewModel>().ReverseMap();
        }
    }
}
