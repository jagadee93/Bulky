
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
            _appDbContext.Products.Include(u => u.Category).Include(u=>u.CategoryId);
        }


       
        public void Add(T entity)
        {
           dbSet.Add(entity);
        }


        //Category
        public IEnumerable<T> GetAll(string? includeProperties=null)
        {
            IQueryable<T> query = dbSet;

            if(!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ','},StringSplitOptions.RemoveEmptyEntries )) {
                   query= query.Include(includeProp);
                }
            }
            return query.ToList();
        }

        public T GetFirstOrDefault(Expression<Func<T, bool>> filter,string? includeProperties=null)
        {
            IQueryable<T> query = dbSet;
            query=query.Where(filter);
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query=query.Include(includeProp);
                }
            }
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
