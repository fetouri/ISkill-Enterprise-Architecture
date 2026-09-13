using API.Dtos;
using AutoMapper;
using Core.Entity;

namespace API.Mappings
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            // ================= 1. الأقسام (Categories) =================
            CreateMap<Category, CategoryDto>().ReverseMap();

            CreateMap<CategoryDtoRequest, Category>();

            CreateMap<UpdateCategoryRequestDto, Category>();

            // ================= 2. التقييمات (CustomerReviews) =================
            CreateMap<CustomerReview, CustomerReviewRespondDto>().ReverseMap();

            CreateMap<CustomerReviewDto, CustomerReview>();

            // ================= 3. الصور (Images) =================
            CreateMap<ImageUrl, ImageUrlDto>().ReverseMap();

            // ================= 4. الخدمات (ServiceApps) =================
            // أ) التحويل للعرض من ServiceApp إلى ServiceAppDto

            CreateMap<ServiceApp, ServiceAppDto>()
                // قاعدة استخراج أول صورة كصورة رئيسية:
            .ForMember(dest => dest.MainImageUrl,
               opt => opt.MapFrom(src =>
                (src.ImageUrl != null && src.ImageUrl.Any())
                 ? (src.ImageUrl.FirstOrDefault(im => im.mainImageUrl != null) != null
                ? src.ImageUrl.FirstOrDefault(im => im.mainImageUrl != null).mainImageUrl
                : src.ImageUrl.FirstOrDefault().SImageUrl)
            : null))


                .ForMember(dest => dest.ImagUrls,
                           opt => opt.MapFrom(src => src.ImageUrl))
              
                .ForMember(dest => dest.categories,
                           opt => opt.MapFrom(src => src.Category))
                
                .ForMember(dest => dest.UserPhoneNumber,
                           opt => opt.MapFrom(src => src.User != null ? src.User.PhoneNumber : null));
         
            // ب) التحويل عند إضافة خدمة جديدة:
            CreateMap<ServiceAppDtoRequest, ServiceApp>();

            // ج) التحويل عند تعديل الخدمة:
            CreateMap<UpdateServiceAppRequestDto, ServiceApp>();

            // ================= .Auth =================

            CreateMap<User, UserDtoRequest>();
            CreateMap<RegisterRequestDto, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));

        }
    }
}
