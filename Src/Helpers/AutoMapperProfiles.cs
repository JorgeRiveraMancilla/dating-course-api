using AutoMapper;
using dating_course_api.Src.DTOs.Account;
using dating_course_api.Src.DTOs.Connection;
using dating_course_api.Src.DTOs.Group;
using dating_course_api.Src.DTOs.Member;
using dating_course_api.Src.DTOs.Message;
using dating_course_api.Src.DTOs.Photo;
using dating_course_api.Src.DTOs.User;
using dating_course_api.Src.Entities;
using dating_course_api.Src.Extensions;

namespace dating_course_api.Src.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserDto>();
            CreateMap<User, MemberDto>()
                .ForMember(
                    dest => dest.Age,
                    opt => opt.MapFrom(src => src.BirthDate.CalculateAge())
                )
                .ForMember(
                    dest => dest.MainPhoto,
                    opt => opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain))
                );
            CreateMap<Photo, PhotoDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId));
            CreateMap<MemberUpdateDto, User>();
            CreateMap<RegisterDto, User>();
            CreateMap<Message, MessageDto>()
                .ForMember(
                    dest => dest.SenderUserName,
                    opt => opt.MapFrom(src => src.Sender.UserName)
                )
                .ForMember(
                    dest => dest.RecipientUserName,
                    opt => opt.MapFrom(src => src.Recipient.UserName)
                );
            CreateMap<DateTime, DateTime>()
                .ConvertUsing(d => DateTime.SpecifyKind(d, DateTimeKind.Utc));
            CreateMap<DateTime?, DateTime?>()
                .ConvertUsing(d =>
                    d.HasValue ? DateTime.SpecifyKind(d.Value, DateTimeKind.Utc) : null
                );
            CreateMap<CreatePhotoDto, Photo>();
            CreateMap<MemberUpdateDto, UpdateUserDto>();
            CreateMap<Group, GroupDto>();
            CreateMap<Connection, ConnectionDto>();
            CreateMap<CreateGroupDto, Group>();
        }
    }
}
