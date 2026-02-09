using BulkyNTier.DataAccess.Repository.IRepository;
using BulkyNTier.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyNTier.DataAccess.Repository
{
    public class ProductImageRepository : Repository<ProductImage>, IProductImageRepository
    {
        private  readonly AppDbContext _appDbContext;
        public ProductImageRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            appDbContext=_appDbContext;
        }

        void Update(ProductImage obj)
        {
            _appDbContext.ProductImages.Update(obj);
        }

    }
}
