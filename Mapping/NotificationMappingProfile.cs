using AutoMapper;
using QuranSchool.Api.DTOs.Notification;
using QuranSchool.Api.Models;

namespace QuranSchool.Api.Mapping;

/// <summary>ملفات AutoMapper للإشعارات: كيانات → DTO.</summary>
public class NotificationMappingProfile : Profile
{
    public NotificationMappingProfile()
    {
        CreateMap<AppNotification, NotificationDto>()
            .ForMember(d => d.SenderRole, o => o.MapFrom(s => s.SenderType));

        CreateMap<AppNotification, NotificationInboxItemDto>()
            .ForMember(d => d.SenderRole, o => o.MapFrom(s => s.SenderType))
            .ForMember(d => d.ReceiverId, o => o.Ignore())
            .ForMember(d => d.IsRead, o => o.Ignore())
            .ForMember(d => d.ReadAt, o => o.Ignore())
            .ForMember(d => d.IsArchived, o => o.Ignore());

        CreateMap<AppNotification, NotificationSentItemDto>()
            .ForMember(d => d.SenderRole, o => o.MapFrom(s => s.SenderType))
            .ForMember(d => d.RecipientsCount, o => o.Ignore())
            .ForMember(d => d.ReadCount, o => o.Ignore());

        CreateMap<AppNotification, NotificationDetailDto>()
            .ForMember(d => d.SenderRole, o => o.MapFrom(s => s.SenderType))
            .ForMember(d => d.Recipients, o => o.MapFrom(s => s.Receivers));

        CreateMap<NotificationReceiver, RecipientDto>()
            .ForMember(d => d.ReceiverRole, o => o.MapFrom(s => s.UserType))
            .ForMember(d => d.ReceiverId, o => o.MapFrom(s => s.UserId))
            .ForMember(d => d.ReceiverName, o => o.MapFrom(s => s.UserFullName));
    }
}
