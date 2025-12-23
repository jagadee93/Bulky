using BulkyNTier.Models;


namespace BulkyNTier.DataAccess.Repository.IRepository
{
    public interface ICompanyRepository:IRepository<Company>
    {
        void Update(Company companyObj);
    }
}
