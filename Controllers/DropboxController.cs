using FiDa.Database;
using FiDa.DatabaseModels;
using FiDa.Lib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Dropbox.Api;

namespace FiDa.Controllers
{
    public class DropboxController : Controller
    {
        private readonly Account _currentUser;
        private readonly UserHost _userHost;
        private readonly FiDaDatabase _db = new();
        private readonly DropboxClient _dropboxClient;

        public DropboxController(IHttpContextAccessor httpContextAccessor)
        {
            _currentUser = Utils.GetAccount(httpContextAccessor.HttpContext?.User.Identity?.Name);
            _userHost = _currentUser.ConfiguredHosts.FirstOrDefault((h) => h.Host == Hosts.Dropbox)!;

            if (null == _userHost) throw new Exception("No Host configured for Dropbox");

            _dropboxClient = new(_userHost.ApiKey);
        }

        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<ActionResult> SyncRepo()
        {
            var dropBoxContent = _dropboxClient.Files.ListFolderAsync(new Dropbox.Api.Files.ListFolderArg(path: null, recursive: true)).Result;

            var uploadedFiles = dropBoxContent.Entries.Select((e) => new UploadedFile
            {
                Account = _currentUser,
                Created = (DateTime)e.AsFile.FileLockInfo.Created!,
                FileId = !e.IsFolder ? e.AsFile.Id : e.AsFolder.Id,
                FileName = e.Name,
                Host = _userHost,
                IsFolder = e.IsFolder,
                Modified = e.AsFile.ServerModified,
                ParentFolderId = e.ParentSharedFolderId,
                Size = e.AsFile.Size,
            });

            await _db.UploadedFiles.AddRangeAsync(uploadedFiles);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}