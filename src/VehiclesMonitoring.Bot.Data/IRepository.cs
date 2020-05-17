using System.Collections.Generic;
using System.Threading.Tasks;

namespace VehiclesMonitoring.Bot.Data
{
    public interface IRepository<T>
    {
        Task InsertAsync(T item);

        Task UpdateAsync(T item);

        Task DeleteAsync(string id);

        Task<T> GetByIdAsync(string id);

        Task<IEnumerable<T>> GetAllAsync();
    }
}