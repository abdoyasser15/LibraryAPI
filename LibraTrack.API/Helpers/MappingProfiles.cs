using AutoMapper;
using Library.Core.Entities;
using LibraTrack.API.DTOs;

namespace LibraTrack.API.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Book,BookToReturnDto>()
                .ForMember(B=>B.Category,O=>O.MapFrom(b=>b.Category.Name))
                .ForMember(B=>B.CategoryId,O=>O.MapFrom(b=>b.Category.Id))
                .ForMember(B=>B.CoverImageUrl,O=>O.MapFrom<BookPictureUrlResolver>());

            CreateMap<CreateBookDto, Book>();

            CreateMap<UpdateBookDto, Book>();

            CreateMap<Category, CategoryToReturnDto>()
                .ForMember(B => B.Category, O => O.MapFrom(c => c.Name))
                .ReverseMap();

            CreateMap<CreateCategoryDto, Category>();

            CreateMap<UpdateCategoryDto, Category>().ReverseMap();


        }
    }
}
