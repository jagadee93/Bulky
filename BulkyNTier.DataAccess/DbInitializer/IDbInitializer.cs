

namespace BulkyNTier.DataAccess.DbInitializer
{
    public interface IDbInitializer
    {
        Task InitializeAsync(string email, string userName, string password);
    }
}

