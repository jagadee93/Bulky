using BulkyNTier.DataAccess.Repository.IRepository;
using BulkyNTier.Models;


namespace BulkyNTier.DataAccess.Repository
{
    public class ApplicationUserRepository:Repository<ApplicationUser>, IApplicationUserRepository
    {

        private readonly AppDbContext _appDbContext;


        public ApplicationUserRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _appDbContext = appDbContext;
        }

    }
}
