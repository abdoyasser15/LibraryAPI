using AutoMapper;
using Library.Core.Entities;
using LibraTrack.API.DTOs;

namespace LibraTrack.API.Helpers
{
    public class BookPictureUrlResolver : IValueResolver<Book,BookToReturnDto, string>
    {
        private readonly IConfiguration configuration;

        public BookPictureUrlResolver(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string Resolve(Book source, BookToReturnDto destination, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.CoverImageUrl))
                return $"{configuration["ApiBaseUrl"]}/{source.CoverImageUrl}";
            return string.Empty;
        }
    }
}
