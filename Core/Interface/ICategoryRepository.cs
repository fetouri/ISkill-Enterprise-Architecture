using Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interface
{
    public interface ICategoryRepository
    {
        Task<Category> CreateAsync(Category category);
        Task<IReadOnlyList<Category>> GetCategorylist();
        Task<Category?> GetById(int id);
        Task<Category?> UpdateAsync(Category category);
        Task<Category?> DeletAsync(int id);

    }
}
