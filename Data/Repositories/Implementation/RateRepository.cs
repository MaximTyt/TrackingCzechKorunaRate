using Data.Repositories.Abstract;
using Entities.Entity;
using Entities.Entity.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace Data.Repositories.Implementation
{
    public class RateRepository : IRepository<RateEntity>
    {
        private readonly IDbContextFactory<DatabaseContext> _contextFactory;
        public RateRepository(IDbContextFactory<DatabaseContext> contextFactory)
        {
            this._contextFactory = contextFactory;
        }
        public async void Create(RateEntity item)
        {
            using (var db = _contextFactory.CreateDbContext())
            {
                await db.Rates.AddAsync(item);
                await db.SaveChangesAsync();
            }
        }

        public async void DeleteById(int id)
        {
            using (var db = _contextFactory.CreateDbContext())
            {
                RateEntity rate = db.Rates.ElementAt(id);
                if (rate != null)
                {
                    db.Rates.Remove(rate);
                    await db.SaveChangesAsync();
                }
            }
        }

        public async Task<ICollection<RateEntity>> Find(Expression<Func<RateEntity, bool>> predicate)
        {
            using (var db = _contextFactory.CreateDbContext())
            {
                return await db.Rates.Where(predicate).ToListAsync();
            }
        }

        public async Task<RateEntity> GetById(int id)
        {
            using (var db = _contextFactory.CreateDbContext())
            {
                return await db.Rates.ElementAtAsync(id);
            }
        }

        public void Update(RateEntity item)
        {
            using (var db = _contextFactory.CreateDbContext())
            {
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
    }
}
