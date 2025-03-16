using AutoMapper;
using WordWise.Api.Models.Domain;
using WordWise.Api.Models.Dto.FlashCard;
using WordWise.Api.Models.Dto.User;

namespace WordWise.Api.Mapping
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            // FlashCard
            CreateMap<CreateFlashCard, Flashcard>().ReverseMap();

            // User
            CreateMap<ExtendedIdentityUser, RegisterDto>().ReverseMap();
        }
    }
}
