using BulkyNTier.DataAccess.Repository.IRepository;
using BulkyNTier.Models;
namespace BulkyNTier.DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository//inheritence
    {
        private readonly AppDbContext _appDbContext;
        public CategoryRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            
                _appDbContext = appDbContext;
            
        }

     

        public void Update(Category obj)
        {
            _appDbContext.Categories.Update(obj);

        }
    }
}
