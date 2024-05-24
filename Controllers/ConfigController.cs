using FiDa.Database;
using FiDa.DatabaseModels;
using FiDa.Lib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pcloud_sdk_csharp.Controllers;
using pcloud_sdk_csharp.Requests;

namespace FiDa.Controllers
{
    public class ConfigController : Controller
    {
        private readonly FiDaDatabase _db = new();
        private readonly Account _currentUser;

        public ConfigController(IHttpContextAccessor contextAccessor)
        {
            _currentUser = Utils.GetAccount(contextAccessor.HttpContext?.User.Identity?.Name);
        }

        [Authorize]
        public ActionResult PCloud()
        {
            return View(_currentUser);
        }

        [HttpPost]
        [Authorize]
        public ActionResult PCloud_Authenticate(IFormCollection formData)
        {
            string client_id = formData["client_id"]!;
            string client_secret = formData["client_secret"]!;
            TempData["client_id"] = client_id;
            TempData["client_secret"] = client_secret;

            var baseUri = $"{Request.Scheme}://{Request.Host}/config/pcloud_authenticate_redirect";
            var authReq = new AuthorizeRequest(client_id, AuthorizeRequest.ResponseType.code, client_secret, baseUri);
            var oauthUrl = AuthController.GetOAuthUrl(authReq);

            var redirect = Redirect(oauthUrl.ToString());
            redirect.PreserveMethod = true;
            redirect.Permanent = false;

            return redirect;
        }

        [HttpPost]
        [Authorize]
        public ActionResult PCloud_Authenticate_Redirect()
        {
            var query = Url.ActionContext.HttpContext.Request.Query.ToDictionary(x => x.Key, x => x.Value);
            var code = query.GetValueOrDefault("code").ToString();
            if (null == code) throw new ArgumentNullException(nameof(code));
            //var location = query.GetValueOrDefault("location");

            var access_token = AuthController.GetOAuth_Token((string)TempData["client_id"], (string)TempData["client_secret"], code)?.Result?.access_token;

            if (null == access_token) throw new ArgumentNullException(nameof(access_token));

            _currentUser.ConfiguredHosts.Add(new UserHost
            {
                Account = _currentUser,
                AccountId = _currentUser.Id,
                ApiKey = access_token,
                Host = Hosts.PCloud
            });
            _db.Account.Update(_currentUser);
            _db.SaveChangesAsync().Wait();

            return RedirectToActionPermanent("Index", "PCloud");
        }
    }
}
