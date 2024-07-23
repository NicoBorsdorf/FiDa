using Dropbox.Api;
using FiDa.DatabaseModels;
using FiDa.ViewModels;
using Microsoft.AspNetCore.Mvc;
using pcloud_sdk_csharp.Client;

namespace FiDa.Views.Shared.Components.StorageUsage
{
    [ViewComponent(Name = "StorageUsage")]
    public class StorageUsageViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(UserHost host)
        {
            var (free, used) = await GetUsage(host);

            return View("StorageUsage", new StorageUsageModel { Host = host, Free = free, Used = used });
        }

        private static async Task<(double free, double used)> GetUsage(UserHost host)
        {
            if (host == null) throw new ArgumentNullException(nameof(host));

            switch (host.Host)
            {
                case Hosts.PCloud:
                    var _client = new PCloudClient(host.ApiKey, host.HostAddress);
                    var pcloudUsage = await _client.General.UserInfo() ?? throw new Exception("Request to user info endpoint returned null.");
                    if (pcloudUsage.result != 0 && pcloudUsage.error != null) throw new Exception($"API request returned code {pcloudUsage.result} with error: {pcloudUsage.error}");

                    // conversion byte to gigabyte = 1000000000
                    return new(Math.Round((double)pcloudUsage.quota! / 1000000000, 2), Math.Round((double)pcloudUsage.usedquota! / 1000000000, 2));

                case Hosts.Dropbox:
                    var _dbxClient = new DropboxClient(oauth2AccessToken: host.ApiKey, oauth2RefreshToken: host.RefreshToken, oauth2AccessTokenExpiresAt: (DateTime)host.TokenExpiration!, appKey: host.AppKey, appSecret: host.AppSecret);
                    _dbxClient.RefreshAccessToken(null).Wait();
                    var dropboxUsage = await _dbxClient.Users.GetSpaceUsageAsync() ?? throw new Exception("Request to user info returned null.");

                    // conversion byte to gigabyte = 1000000000
                    return new(Math.Round((double)dropboxUsage.Allocation.AsIndividual.Value.Allocated! / 1000000000, 2), Math.Round((double)dropboxUsage.Used! / 1000000000, 2));

                default: return new(1, 0);
            }
        }
    }
}
