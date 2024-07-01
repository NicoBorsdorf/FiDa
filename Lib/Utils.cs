using FiDa.Database;
using FiDa.DatabaseModels;
using Microsoft.EntityFrameworkCore;

namespace FiDa.Lib
{
    public static class Utils
    {
        private static readonly FiDaDatabase _db = new(options: new());

        private static async Task<Account> AddAccount(Account account)
        {
            await _db.AddAsync(account);
            await _db.SaveChangesAsync();
            return account;
        }

        /// <summary>
        /// Gets Account form Database with given username. If user is not found, a new entry will be created and returned.
        /// </summary>
        /// <param name="uName">Name of the user on the DB. Initially received from Auth0.</param>
        /// <param name="includeHosts">Flag if configured hosts should be retrievend from Db.</param>
        /// <returns><see cref="Account">Account</see> object from Database.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="uName"/> is null or empty.</exception>
        public static Account GetAccount(string uName, bool includeHosts = false)
        {
            if (string.IsNullOrEmpty(uName)) throw new ArgumentNullException(nameof(uName));

            if (includeHosts) return _db.Account.Include(a => a.ConfiguredHosts).FirstOrDefaultAsync(a => a.Username == uName).Result ?? AddAccount(new Account(uName)).Result;

            return _db.Account.FirstOrDefault((a) => a.Username == uName) ?? AddAccount(new Account(uName)).Result;
        }
    }
}
