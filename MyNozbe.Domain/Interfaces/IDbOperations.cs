using System.Threading.Tasks;

namespace MyNozbe.Domain.Interfaces
{
    public interface IDbOperations<T>
    {
        Task<int> AddAsync(T model);

        Task UpdateAsync(T model);

        Task<T> GetAsync(int id);
    }
}