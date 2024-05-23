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
        [ValidateAntiForgeryToken]
        public ActionResult PCloud_Authenticate(IFormCollection formData)
        {
            string client_id = formData["client_id"]!;
            string client_secret = formData["client_secret"]!;

            var baseUri = $"{Request.Scheme}://{Request.Host}/config/pcloud_authenticate_redirect";
            var authReq = new AuthorizeRequest(client_id, AuthorizeRequest.ResponseType.code, client_secret, baseUri);
            var oauthUrl = AuthController.GetOAuthUrl(authReq);

            var redirect = Redirect(oauthUrl.ToString());
            redirect.PreserveMethod = true;
            redirect.Permanent = false;

            return redirect;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PCloud_Authenticate_Rdirect()
        {
            var query = Url.ActionContext.HttpContext.Request.Query.ToDictionary(x => x.Key, x => x.Value);
            var code = query.GetValueOrDefault("code").ToString();
            if (null == code) throw new ArgumentNullException("No code found in query");
            //var location = query.GetValueOrDefault("location");

            var (access_token) = AuthController.GetAccessToken(new { client_id = });

        }
    }
}
