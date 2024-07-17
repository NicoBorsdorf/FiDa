using FiDa.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FiDa.DatabaseModels;
using FiDa.ViewModels;
using FiDa.Lib;
using pcloud_sdk_csharp.File.Requests;
using pcloud_sdk_csharp.Client;
using pcloud_sdk_csharp.Folder.Requests;

namespace FiDa.Controllers
{
    [Route("pcloud/[action]")]
    public class PCloudController : Controller
    {
        private readonly FiDaDatabase _db = new(options: new());
        private readonly Account _currentUser;
        private readonly UserHost _host;
        private readonly PCloudClient _pClient;
        private readonly ILogger _logger;

        public PCloudController(IHttpContextAccessor contextAccessor, ILogger<PCloudController> logger)
        {
            _logger = logger;
            _logger.LogInformation("New Instance PCloudController");

            ViewData["Title"] = "PCloud";

            _currentUser = Utils.GetAccount(contextAccessor.HttpContext?.User.Identity?.Name!);

            _host = _currentUser.ConfiguredHosts.FirstOrDefault((h) => h.Host == Hosts.PCloud)!;
            if (null == _host)
            {
                _logger.LogError("PCloudController - could not find host configuration for user: {user}", _currentUser.Username);
                throw new Exception("No PCloud host configured for user");
            }

            // throw if token is null is already implemented in SDK
            _logger.LogInformation("PCloudController - create pcloud api client");
            _pClient = new PCloudClient(_host.ApiKey, _host.HostAddress);
        }


        [Authorize]
        public ActionResult Index(long? id)
        {
            _logger.LogInformation("PCloudController Index - folderId: {folderId}", id);

            ViewBag.Account = _currentUser;

            var _files = _db.UploadedFiles.Where((f) => f.Account == _currentUser && f.Host == _host);
            var fileViewModel = new FileViewModel
            {
                RootFiles = _files.Where((f) => f.ParentFolderId == (id ?? 0).ToString()).OrderByDescending((f) => f.IsFolder).ToList(),
                Folders = _files.Where((f) => f.IsFolder).ToList()
            };
            var model = new BaseViewModel
            {
                Account = _currentUser,
                FileViewModel = fileViewModel
            };
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> UploadFile(IFormCollection? form)
        {
            _logger.LogInformation("PCloudController UploadFile - formCount: {count}", form?.Count);

            if (form != null && form.Count > 0)
            {
                if (!_db.UploadedFiles.Where((f) => f.Account == _currentUser && f.Host == _host).Any()) await SyncRepo();

                long folderId = long.Parse(form["Folder"]!);
                IFormFile[] files = form.Files.ToArray();

                List<string> errors = new();

                foreach (var file in files)
                {
                    try
                    {
                        UploadFileRequest req = new(folderId, file.FileName, file.OpenReadStream());
                        var res = await _pClient.Files.UploadFile(req);

                        if (null == res || (res.result != 0 && res.error != null)) throw new Exception("PCloud returned an exception: " + res?.error);

                        var fileMeta = res.metadata!.First();

                        _db.UploadedFiles.AddRangeAsync(new UploadedFile
                        {
                            AccountId = _currentUser.Id,
                            Account = _currentUser,
                            FileName = file.FileName,
                            HostId = _host.Id,
                            Host = _host,
                            FileId = res.fileids!.First().ToString(),
                            Size = !fileMeta.isfolder ? fileMeta.size / 1000 : null,
                            ParentFolderId = folderId.ToString(),
                            IsFolder = false,
                            Modified = fileMeta.modified,
                            Created = fileMeta.created
                        }).Wait();

                        _db.Account.Update(_currentUser);
                        _db.UserHost.Update(_host);
                    }
                    catch (Exception e)
                    {
                        Console.Error.WriteLine(e.Message);
                        errors.Add($"File upload failed for file: {file.FileName}");
                    }
                }
                _db.SaveChangesAsync().Wait();

                TempData["Errors"] = errors;
            }
            return RedirectToAction("Index");
        }

        // Syncs all information from pCloud to App
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> SyncRepo()
        {
            _logger.LogInformation("PCloudController SyncRepo");

            // Clear current db entries for pcloud host
            var existingFiles = _db.UploadedFiles.Where(f => f.AccountId == _currentUser.Id && f.Host == _host);
            _db.UploadedFiles.RemoveRange(existingFiles);

            ListFolderRequest req = new(folderId: 0, recursive: true);
            var res = _pClient.Folders.ListFolder(req).Result;

            if (res == null || (res.result != 0 && res.error != null)) throw new Exception("Could not Syncronize: " + res?.error);

            var contents = res.metadata?.contents;

            if (contents != null)
            {
                var inserts = contents.Select(meta => new UploadedFile
                {
                    AccountId = _currentUser.Id,
                    Account = _currentUser,
                    FileName = meta.name,
                    HostId = _host.Id,
                    Host = _host,
                    FileId = meta.isfolder ? meta.folderid.ToString()! : meta.fileid.ToString()!,
                    ParentFolderId = meta.parentfolderid.ToString(),
                    Size = meta.size / 1000,
                    IsFolder = meta.isfolder,
                    Modified = meta.modified,
                    Created = meta.created
                });

                _db.Account.Update(_currentUser);
                _db.UserHost.Update(_host);
                await _db.UploadedFiles.AddRangeAsync(inserts);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}