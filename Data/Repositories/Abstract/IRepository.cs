using Entities.Entity;
using System.Linq.Expressions;

namespace Data.Repositories.Abstract
{
    public interface IRepository<T> where T : class
    {
        Task<ICollection<RateEntity>> Find(Expression<Func<RateEntity, bool>> predicate);
        Task<T> GetById(int id);
        void Create(T item);
        void Update(T item);
        void DeleteById(int id);
    }
}
