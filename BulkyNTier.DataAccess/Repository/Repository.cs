
using BulkyNTier.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

using System.Linq.Expressions;
namespace BulkyNTier.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {

        private readonly AppDbContext _appDbContext;
        internal DbSet<T> dbSet; //we are adding this because we cant use db.categories.Add() like this we are in a generic class
       

        public Repository(AppDbContext appDbContext)
        {

            _appDbContext = appDbContext;
            this.dbSet = _appDbContext.Set<T>();
            //db.categories== dbSet
        }


       
        public void Add(T entity)
        {
           dbSet.Add(entity);
        }

        public IEnumerable<T> GetAll()
        {
            IQueryable<T> query = dbSet;
            return query.ToList();
        }

        public T GetFirstOrDefault(Expression<Func<T, bool>> filter)
        {
            IQueryable<T> query = dbSet;
            query=query.Where(filter);
            return query.FirstOrDefault();
            
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            dbSet.RemoveRange(entities);
        }

        public void Update(T entity)
        {
            dbSet.Update(entity);
        }
    }
}
