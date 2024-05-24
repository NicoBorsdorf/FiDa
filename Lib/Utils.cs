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

        public static Account GetAccount(string? uName)
        {
            if (string.IsNullOrEmpty(uName)) throw new ArgumentNullException(nameof(uName));
            var a = _db.Account.FirstOrDefault((a) => a.Username == uName);
            if (a == null) throw new ArgumentException("No Account found on Database");
            return a;
        }
    }
}
