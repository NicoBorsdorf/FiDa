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
            return await _db.Account.FirstAsync(a => a == account);
        }

        /// <summary>
        /// Gets Account form Database with given username. If user is not found, a new entry will be created and returned.
        /// </summary>
        /// <param name="uName">Name of the user on the DB. Initially received from Auth0.</param>
        /// <param name="includeHosts">Flag if configured hosts should be retrievend from Db.</param>
        /// <returns><see cref="Account">Account</see> object from Database.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="uName"/> is null or empty.</exception>
        public static Account GetAccount(string uName)
        {
            if (string.IsNullOrEmpty(uName)) throw new ArgumentNullException(nameof(uName));

            return _db.Account.Include(a => a.ConfiguredHosts).FirstOrDefault((a) => a.Username == uName) ?? AddAccount(new Account(uName)).Result;
        }

        /// <summary>
        /// Deletes a user host from database
        /// </summary>
        /// <param name="host">User host to remove.</param>
        /// <param name="_logger">Logger of corresponding controller.</param>
        /// <returns>Bool if successfull or not.</returns>
        public static async Task<bool> DeleteHost(UserHost host, ILogger _logger)
        {
            _logger.LogInformation("Utils - DeleteHost");
            try
            {
                var toRemove = await _db.UserHost.FindAsync(host.Id);
                if (toRemove == null)
                {
                    _logger.LogError("Host {host} of {name} not found", host.Host, host.Account.Username);
                    return false;
                }
                _db.UserHost.Remove(toRemove);
                await _db.SaveChangesAsync();

                _logger.LogInformation("Host {host} of user {name} has been removed.", host.Host, host.Account.Username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while removing the host");
                return false;
            }

            return true;
        }
    }
}
