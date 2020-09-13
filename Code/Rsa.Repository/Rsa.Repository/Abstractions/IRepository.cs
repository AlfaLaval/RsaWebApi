using System.Linq;
using System.Threading.Tasks;

namespace Rsa.Repository.Abstractions
{
    public interface IRepository
    {
        IQueryable<T> GetAll<T>();
        void Add<T>(T entity);
        void Update<T>(T entity);

    }
}
