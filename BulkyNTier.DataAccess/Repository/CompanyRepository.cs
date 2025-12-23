using BulkyNTier.DataAccess.Repository.IRepository;
using BulkyNTier.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyNTier.DataAccess.Repository
{
    public class CompanyRepository:Repository<Company>,ICompanyRepository
    {

        public readonly AppDbContext _appDbContext;

        public CompanyRepository(AppDbContext appDbContext):base(appDbContext)
        {
            _appDbContext = appDbContext;
        }


        public void Update(Company companyObj)
        {
            _appDbContext.Companies.Update(companyObj);
        }
    
    }
}
