using Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interface
{
    public interface IImageUrlRepository
    {
        Task<ImageUrl> CreateImageUrlAsync(ImageUrl imageUrl);
        Task<IReadOnlyList<ImageUrl>> GetImageUrllist();
        public ImageUrl? GetMainImageUrlByServiceAppId(int serviceAppId);
        //Task<IReadOnlyList<ImageUrl>> GetMainImageUrlByServiceAppId(int serviceAppId);
        Task<ImageUrl?> GetImageUrlById(int id);
        Task<ImageUrl?> UpdateImageUrlAsync(ImageUrl imageUrl);
        Task<ImageUrl?> DeleteImageUrlAsync(int id);

    }
}
