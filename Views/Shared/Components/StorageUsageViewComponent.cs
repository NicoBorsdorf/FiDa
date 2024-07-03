using FiDa.DatabaseModels;
using FiDa.ViewModels;
using Microsoft.AspNetCore.Mvc;
using pcloud_sdk_csharp.Client;

namespace FiDa.Views.Shared.Components
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
                    var _client = new PCloudClient(host.ApiKey, new Uri("https://eapi.pcloud.com/"));
                    var res = await _client.General.UserInfo() ?? throw new Exception("Request to user info endpoint returned null.");
                    if (res.result != 0 && res.error != null) throw new Exception($"API request returned code {res.result} with error: {res.error}");

                    // conversion byte to gigabyte == 1000000000
                    return new(Math.Round((double)res.quota! / 1000000000, 2), Math.Round((double)res.usedquota! / 1000000000, 2));
                default: return new(0, 0);
            }


        }
    }
}
