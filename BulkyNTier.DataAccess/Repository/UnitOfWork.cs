using BulkyNTier.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyNTier.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        public ICategoryRepository CategoryRepository {  get; set; }
        public IProductRepository ProductRepository { get; set; }
        public ICompanyRepository CompanyRepository { get; set; }
        private readonly AppDbContext _db;
        public UnitOfWork(AppDbContext db) {

            _db = db;
            CategoryRepository = new CategoryRepository(_db);
            ProductRepository=new ProductRepository(_db);
            CompanyRepository=new CompanyRepository(_db);
           
        }

        
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
