using Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interface
{
    public interface IServiceAppRepository
    {
        Task<ServiceApp> CreateAsync(ServiceApp serviceApp);
        Task<IReadOnlyList<ServiceApp>> GetServiceApplist(
            string? query = null,
            string? sortBy = null,
            string? sortDirection = null,
            int? pageNumber = 1,
            int? pageSize = 6);
        Task<ServiceApp?> GetServiceAppById(int id);
        Task<ServiceApp?> GetServiceAppByUrlHandleAsync(string urlHandle);
        Task<ServiceApp?> UpdateServiceAppAsync(ServiceApp serviceApp);
        Task<ServiceApp?> DeleteServiceAppAsync(int id);
        Task<IReadOnlyList<ServiceApp>> GetServiceAppsByUserId(
            string userId,
    string? query = null,
    string? sortBy = null,
    string? sortDirection = null,
    int? pageNumber = 1,
    int? pageSize = 10);

        Task<int> GetCount();

    }
}

