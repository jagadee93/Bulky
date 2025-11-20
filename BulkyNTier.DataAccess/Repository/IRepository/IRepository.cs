
using System.Linq.Expressions;


namespace BulkyNTier.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        //T-category
        IEnumerable<T> GetAll(string? includeProperties);

        T GetFirstOrDefault(Expression<Func<T,bool>> filter,string? includeProperties);
        void Add(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        //IEnumerable<T> Find(int id);
        //IEnumerable<T> FindAll();

    }
}
