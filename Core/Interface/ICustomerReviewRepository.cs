using Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interface
{
    public interface ICustomerReviewRepository
    {
        Task<CustomerReview> CreateCustomerReviewAsync(CustomerReview customerReview);
        Task<IReadOnlyList<CustomerReview>> GetCustomerReviewlist();
        Task<CustomerReview?> GetCustomerReviewById(int id);
        Task<CustomerReview?> UpdateCustomerReviewAsync(CustomerReview customerReview);
        Task<CustomerReview?> DeleteCustomerReviewAsync(int id);

    }
}
