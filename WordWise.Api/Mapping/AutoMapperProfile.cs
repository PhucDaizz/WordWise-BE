using AutoMapper;
using WordWise.Api.Models.Domain;
using WordWise.Api.Models.Dto.FlashCard;
using WordWise.Api.Models.Dto.FlashcardReview;
using WordWise.Api.Models.Dto.FlashCardSet;
using WordWise.Api.Models.Dto.User;

namespace WordWise.Api.Mapping
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            // FlashCard
            CreateMap<FlashCardDto, Flashcard>().ReverseMap();
            CreateMap<CreateFlashCard, Flashcard>().ReverseMap();
            CreateMap<UpdateFlashCard, Flashcard>().ReverseMap();
            CreateMap<CreateRangeFlashcardDto, Flashcard>().ReverseMap();

            // User
            CreateMap<ExtendedIdentityUser, RegisterDto>().ReverseMap();
            CreateMap<ExtendedIdentityUser, UserDto>().ReverseMap();

            // FlashcardReview
            CreateMap<FlashcardReview, FlashcardReviewDto>().ReverseMap();

            // FlashcardSet
            CreateMap<CreateFlashCardSetDto, FlashcardSet>().ReverseMap();
            CreateMap<FlashcardSet, FlashCardSetDto>()
                .ForMember(dest => dest.Flashcards, opt => opt.MapFrom(dest => dest.Flashcards))
                .ForMember(dest => dest.User, opt => opt.MapFrom(dest => dest.User))
                .ForMember(dest => dest.flashcardReviews, opt => opt.MapFrom(dest => dest.FlashcardReviews));
            CreateMap<UpdateFlashcardSet, FlashcardSet>().ReverseMap();
                
            // FlashcardReview
            CreateMap<CreateFlashcardReviewDto, FlashcardReview>().ReverseMap();
        }
    }
}
