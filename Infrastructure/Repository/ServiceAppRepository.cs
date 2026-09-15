using Core.Entity;
using Core.Interface;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class ServiceAppRepository : IServiceAppRepository
    {

        private readonly SkillDbContext _db;

        public ServiceAppRepository(SkillDbContext db)
        {
            _db = db;
        }


        public async Task<ServiceApp> CreateAsync(ServiceApp serviceApp)
        {
            await _db.Services.AddAsync(serviceApp);
            await _db.SaveChangesAsync();
            return serviceApp;
        }

        public async Task<ServiceApp?> DeleteServiceAppAsync(int id)
        {
            var serviceApp = await _db.Services.FindAsync( id);
            if (serviceApp != null)
            {

                _db.Services.Remove(serviceApp);
                await _db.SaveChangesAsync();
                return serviceApp;
            }
            return null;
        }

        public async Task<int> GetCount()
        {
           return await _db.Services.CountAsync();
        }

        public async Task<ServiceApp?> GetServiceAppById(int id)
        {
            return await _db.Services.
                Include(c => c.Category)
                .Include(u => u.User)
               .Include(r => r.CustomerReviews)
                .Include(im => im.ImageUrl).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<ServiceApp?> GetServiceAppByUrlHandleAsync(string urlHandle)
        {
            return await _db.Services.
                           Include(c => c.Category)
                           .Include(u => u.User)
                           .Include(r => r.CustomerReviews)
                           .Include(im => im.ImageUrl).FirstOrDefaultAsync(x => x.UrlHandle == urlHandle );
        }

        public async Task<IReadOnlyList<ServiceApp>> GetServiceApplist(
    string? query = null,
    string? sortBy = null,
     string? sortDirection = null,
     int? pageNumber = 1,
     int? pageSize = 6)
        {
            // Query
            var serviceApps = _db.Services.AsQueryable();

            // Filtering
            if (string.IsNullOrWhiteSpace(query) == false)
            {
                serviceApps = serviceApps.Where(x => x.Name.Contains(query));
            }


            // Sorting
            if (string.IsNullOrWhiteSpace(sortBy) == false)
            {
                if (string.Equals(sortBy, "Name", StringComparison.OrdinalIgnoreCase))
                {
                    var isAsc = string.Equals(sortDirection, "asc", StringComparison.OrdinalIgnoreCase)
                        ? true : false;
                    serviceApps = isAsc ? serviceApps.OrderBy(x => x.Name) : serviceApps.OrderByDescending(x => x.Name);
                }

            }

            // Pagination
            var skipResults = (pageNumber - 1) * pageSize;
            serviceApps = serviceApps.Skip(skipResults ?? 0).Take(pageSize ?? 100);

            return await serviceApps
                            .AsNoTracking()
                            .Include(c => c.Category)
                            .Include(u => u.User)
                            .Include(r => r.CustomerReviews)
                            .Include(im => im.ImageUrl) // 👈 هذا السطر فقط هو الذي كان ينقصك
                            .ToListAsync();
        }


        //public async Task<IReadOnlyList<ServiceApp>> GetServiceApplist(
        //    string? query = null,
        //    string? sortBy = null,
        //     string? sortDirection = null,
        //     int? pageNumber = 1,   
        //     int? pageSize = 6)
        //{
        //    // Query
        //    var serviceApps = _db.Services.AsQueryable();

        //    // Filtering
        //    if (string.IsNullOrWhiteSpace(query) == false)
        //    {
        //        serviceApps = serviceApps.Where(x => x.Name.Contains(query));
        //    }


        //    // Sorting
        //    if (string.IsNullOrWhiteSpace(sortBy) == false)
        //    {
        //        if (string.Equals(sortBy, "Name", StringComparison.OrdinalIgnoreCase))
        //        {
        //            var isAsc = string.Equals(sortDirection, "asc", StringComparison.OrdinalIgnoreCase)
        //                ? true : false;
        //            serviceApps = isAsc ? serviceApps.OrderBy(x => x.Name) : serviceApps.OrderByDescending(x => x.Name);
        //        }

        //    }

        //    // Pagination
        //    // Pagenumber 1 pagesize 5 - skip 0, take 5
        //    // Pagenumber 2 pagesize 5 - skip 5, take 5, [6,7,8,9,10]
        //    // Pagenumber 3 pagesize 5 - skip 10, take 5

        //    var skipResults = (pageNumber - 1) * pageSize;
        //    serviceApps = serviceApps.Skip(skipResults ?? 0).Take(pageSize ?? 100);

        //    return await serviceApps.
        //                    Include(c => c.Category)
        //                   .Include(u => u.User)
        //                   .Include(r => r.CustomerReviews)
        //                                             .ToListAsync();
        //}

        public async Task<IReadOnlyList<ServiceApp>> GetServiceAppsByUserId(string userId, string? query = null, string? sortBy = null, string? sortDirection = null, int? pageNumber = 1, int? pageSize = 10)
        {
            // إنشاء استعلام قاعدة البيانات
            var serviceApps = _db.Services.AsQueryable();

            // تصفية الخدمات بناءً على معرف المستخدم
            serviceApps = serviceApps.Where(x => x.UserId == userId);

            // تطبيق البحث إذا كان متاحًا
            if (!string.IsNullOrWhiteSpace(query))
            {
                serviceApps = serviceApps.Where(x => x.Name.Contains(query) || x.Description.Contains(query));
            }

            //if (!string.IsNullOrWhiteSpace(query))
            //{
            //    serviceApps = serviceApps.Where(x => x.Name.Contains(query) || x.Category.Name.Contains(query));
            //}

            // تطبيق الفرز إذا كان متاحًا
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                var isAsc = string.Equals(sortDirection, "asc", StringComparison.OrdinalIgnoreCase);
                serviceApps = isAsc
                    ? serviceApps.OrderBy(x => EF.Property<object>(x, sortBy))
                    : serviceApps.OrderByDescending(x => EF.Property<object>(x, sortBy));
            }

            // الترقيم (Pagination)
            var skipResults = (pageNumber - 1) * pageSize;
            serviceApps = serviceApps.Skip(skipResults ?? 0).Take(pageSize ?? 6);

            // إرجاع البيانات مع العلاقات المرتبطة
            return await serviceApps
                .Include(c => c.Category)
                .Include(u => u.User)
                .Include(r => r.CustomerReviews)
                .Include(im => im.ImageUrl)
                .ToListAsync();
        }

        public  async Task<ServiceApp?> UpdateServiceAppAsync(ServiceApp serviceApp)
        {
            var existingserviceapp = await _db.Services.FindAsync( serviceApp.Id);

            if (existingserviceapp != null)
            {
                _db.Entry(existingserviceapp).CurrentValues.SetValues(serviceApp);
                await _db.SaveChangesAsync();
                return existingserviceapp;
            }

            return null;

        }

      
    }
}       

