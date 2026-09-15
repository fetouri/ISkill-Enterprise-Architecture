using Core.Entity;
using Core.Interface;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class CustomerReviewRepository : ICustomerReviewRepository
    {
        private readonly SkillDbContext _db;

        public CustomerReviewRepository(SkillDbContext db)
        {
            _db = db;
        }

        public async Task<CustomerReview> CreateCustomerReviewAsync(CustomerReview customerReview)
        {

            await _db.CustomerReviews.AddAsync(customerReview);
            await _db.SaveChangesAsync();
            return customerReview;
        }

        public async Task<CustomerReview?> DeleteCustomerReviewAsync(int id)
        {
            var IT = await _db.CustomerReviews.FirstOrDefaultAsync(c => c.ReviewId == id);
            if (IT != null)
            {

                _db.CustomerReviews.Remove(IT);
                await _db.SaveChangesAsync();
                return IT;
            }
            return null;
        }

        public async Task<CustomerReview?> GetCustomerReviewById(int id)
        {
            return await _db.CustomerReviews.Include(s => s.serviceApp)
                /*.Include(u => u.User)*/.FirstOrDefaultAsync(x => x.ReviewId == id);
        }

        public async Task<IReadOnlyList<CustomerReview>> GetCustomerReviewlist()
        {
            var c = await _db.CustomerReviews.Include(s => s.serviceApp)
                /*.Include(u => u.User)*/.ToListAsync();
            return c.ToList();
        }

        public async Task<CustomerReview?> UpdateCustomerReviewAsync(CustomerReview customerReview)
        {
            var existingReviews = await _db.CustomerReviews.FirstOrDefaultAsync(x => x.ReviewId == customerReview.ReviewId);

            if (existingReviews != null)
            {
                _db.Entry(existingReviews).CurrentValues.SetValues(customerReview);
                await _db.SaveChangesAsync();
                return customerReview;
            }

            return null;
        
    }
    }
}
