using BulkyNTier.DataAccess.Repository.IRepository;
using BulkyNTier.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyNTier.DataAccess.Repository
{
    public class ProductRepository: Repository<Product>, IProductRepository { 

        private readonly AppDbContext _db;
        public ProductRepository(AppDbContext db) : base(db)
        {
           _db= db;
        }


        public void Update(Product product)
        {
            _db.Products.Update(product);
        }
    }
    
}

