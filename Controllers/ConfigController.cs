using FiDa.Database;
using FiDa.DatabaseModels;
using FiDa.Lib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pcloud_sdk_csharp.Auth.Requests;
using pcloud_sdk_csharp.Auth.Controller;
using FiDa.ViewModels;
using Dropbox.Api;

namespace FiDa.Controllers
{
    public class ConfigController : Controller
    {
        private readonly FiDaDatabase _db = new(options: new());
        private readonly Account _currentUser;
        private readonly ILogger _logger;

        public ConfigController(ILogger<ConfigController> logger, IHttpContextAccessor contextAccessor)
        {
            _logger = logger;
            _logger.LogInformation("New Instance ConfigController");

            ViewData["Title"] = "Config";

            _currentUser = Utils.GetAccount(contextAccessor.HttpContext?.User.Identity?.Name!);
        }

        // PCloud API

        [Authorize]
        public ActionResult PCloud()
        {
            _logger.LogInformation("ConfigController - PCloud");

            var model = new BaseViewModel
            {
                Account = _currentUser
            };
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult PCloud_Authenticate(IFormCollection formData)
        {
            _logger.LogInformation("ConfigController - PCloud_Authenticate - formDataCount: {count}", formData.Count);

            string client_id = formData["client_id"]!;
            string client_secret = formData["client_secret"]!;
            TempData["client_id"] = client_id;
            TempData["client_secret"] = client_secret;

            var redirectUri = $"{Request.Scheme}://{Request.Host}/config/pcloud_authenticate_redirect";
            var authReq = new AuthorizeRequest(client_id, AuthorizeRequest.ResponseType.code, redirectUri);
            var oauthUrl = Authorize.GetOAuthUrl(authReq);

            var redirect = Redirect(oauthUrl.ToString());
            redirect.PreserveMethod = true;
            redirect.Permanent = false;

            return redirect;
        }

        [Authorize]
        public ActionResult PCloud_Authenticate_Redirect()
        {
            _logger.LogInformation("ConfigController - PCloud_Authenticate_Redirect");

            var query = Request.Query.ToDictionary(x => x.Key, x => x.Value);
            var code = query.GetValueOrDefault("code").ToString() ?? throw new ArgumentNullException("code");
            var hostname = query.GetValueOrDefault("hostname").ToString() ?? throw new ArgumentNullException("hostname");
            var hostUri = new Uri("https://" + hostname + "/");

            var access_token = Authorize.GetOAuthToken((string)TempData["client_id"]!, (string)TempData["client_secret"]!, code, hostUri)?.Result?.access_token ?? throw new ArgumentNullException("access_token");

            _currentUser.ConfiguredHosts.Add(new UserHost
            {
                Account = _currentUser,
                AccountId = _currentUser.Id,
                ApiKey = access_token,
                Host = Hosts.PCloud,
                HostAddress = hostUri
            });

            _db.Account.Update(_currentUser);
            _db.SaveChangesAsync().Wait();

            TempData.Clear();

            return RedirectToActionPermanent("Index", "Dashboard");
        }

        // Dropbox api

        [Authorize]
        public ActionResult Dropbox()
        {
            _logger.LogInformation("ConfigController - Dropbox");

            var model = new BaseViewModel
            {
                Account = _currentUser
            };
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult Dropbox_Authenticate(IFormCollection formData)
        {
            _logger.LogInformation("ConfigController - Dropbox_Authenticate - formDataCount: {count}", formData.Count);

            string client_id = formData["client_id"]!;
            string client_secret = formData["client_secret"]!;
            TempData["client_id"] = client_id;
            TempData["client_secret"] = client_secret;

            var redirectUri = $"{Request.Scheme}://{Request.Host}/config/dropbox_authenticate_redirect";
            var oauthUrl = DropboxOAuth2Helper.GetAuthorizeUri(oauthResponseType: OAuthResponseType.Code, clientId: client_id, redirectUri, tokenAccessType: TokenAccessType.Offline);

            return Redirect(oauthUrl.ToString());
        }

        [Authorize]
        public ActionResult Dropbox_Authenticate_Redirect()
        {
            _logger.LogInformation("ConfigController - Dropbox_Authenticate_Redirect");

            var query = Request.Query.ToDictionary(x => x.Key, x => x.Value);
            var code = query.GetValueOrDefault("code").ToString() ?? throw new ArgumentNullException("code");

            var redirectUri = $"{Request.Scheme}://{Request.Host}/config/dropbox_authenticate_redirect";
            var authResponse = DropboxOAuth2Helper.ProcessCodeFlowAsync(code, appKey: (string)TempData["client_id"]!, appSecret: (string)TempData["client_secret"]!, redirectUri).Result ?? throw new Exception("Dropbox code flow returned null.");

            var newUserHost = new UserHost
            {
                Account = _currentUser,
                AccountId = _currentUser.Id,
                ApiKey = authResponse.AccessToken,
                AppKey = (string)TempData["client_id"]!,
                AppSecret = (string)TempData["client_secret"]!,
                TokenExpiration = authResponse.ExpiresAt,
                RefreshToken = authResponse.RefreshToken,
                Host = Hosts.Dropbox,
                HostAddress = new Uri("https://api.dropbox.com")
            };

            _db.UserHost.Add(newUserHost);
            //_currentUser.ConfiguredHosts.Add(newUserHost);

            _db.Account.Update(_currentUser);
            _db.SaveChangesAsync().Wait();

            TempData.Clear();

            return RedirectToActionPermanent("Index", "Dashboard");
        }

    }
}
