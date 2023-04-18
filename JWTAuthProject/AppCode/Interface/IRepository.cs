using JWTAuthProject.Models;

namespace JWTAuthProject.AppCode.Interface
{
    public interface IRepository<T> where T : class
    {
        Task<Response<T>> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync(T entity = null, int loginId = 0);
        Task<Response> AddAsync(T entity);
        Task<Response> DeleteAsync(int id);
        Task<IReadOnlyList<T>> GetDropdownAsync(T entity);
    }
}
