using FiDa.Database;
using FiDa.DatabaseModels;

namespace FiDa.Lib
{
    public static class Utils
    {
        private static readonly FiDaDatabase _db = new();
        public static async Task<Account?> AddAccount(Account account)
        {
            try
            {
                await _db.AddAsync(account);
                await _db.SaveChangesAsync();
                return account;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
