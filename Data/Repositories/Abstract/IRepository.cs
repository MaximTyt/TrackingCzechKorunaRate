using Entities.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

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
