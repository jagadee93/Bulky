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



            var objFromDB=_db.Products.Where(p => p.Id == product.Id).FirstOrDefault();
            if (objFromDB != null)
            {
                objFromDB.Title=product.Title;
                objFromDB.ISBN=product.ISBN;
                objFromDB.Description=product.Description;
                objFromDB.Price=product.Price;
                objFromDB.Price50=product.Price50;
                objFromDB.ListPrice=product.ListPrice;
                objFromDB.Price100=product.Price100;
                
                objFromDB.CategoryId=product.CategoryId;
                objFromDB.Author=product.Author;


                if (product.ImageURL != null)
                {
                    objFromDB.ImageURL=product.ImageURL;
                }

                

            }
            //_db.Products.Update(product);
        }
    }
    
}

