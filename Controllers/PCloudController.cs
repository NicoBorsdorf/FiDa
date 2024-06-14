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
    public class PCloudController : Controller
    {
        private readonly FiDaDatabase _db = new();
        private Account _currentUser;
        private readonly UserHost _host;
        private readonly PCloudClient _pClient;

        public PCloudController(IHttpContextAccessor contextAccessor)
        {
            _currentUser = Utils.GetAccount(contextAccessor.HttpContext?.User.Identity?.Name);

            _host = _currentUser.ConfiguredHosts.FirstOrDefault((h) => h.Host == Hosts.PCloud)!;
            if (null == _host) throw new Exception("No PCloud host configured for user");

            // throw if token is null is already implemented in SDK
            _pClient = new PCloudClient(_host.ApiKey);
        }


        [Authorize]
        [HttpGet]
        public ActionResult Index(long? folderId)
        {
            var _files = _db.UploadedFiles.Where((f) => f.Account == _currentUser && f.Host == _host);
            FileViewModel model = new()
            {
                RootFiles = _files.Where((f) => f.ParentFolderId == (folderId ?? 0).ToString()).OrderByDescending((f) => f.IsFolder).ToList(),
                Folders = _files.Where((f) => f.IsFolder).ToList()
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public ActionResult UploadFile(IFormCollection? form)
        {
            if (form != null && form.Count > 0)
            {
                long folderId = long.Parse(form["Folder"]!);
                IFormFile[] files = form.Files.ToArray();

                List<string> errors = new();

                foreach (var file in files)
                {
                    try
                    {
                        UploadFileRequest req = new(folderId, file.FileName, file.OpenReadStream());
                        var res = _pClient.Files.UploadFile(req).Result;

                        if (null == res || (res.result != 0 && res.error != null)) throw new Exception("PCloud returned an exception: " + res?.error);

                        var fileMeta = res.metadata!.First();

                        _db.UploadedFiles.Add(new UploadedFile
                        {
                            FileName = file.FileName,
                            Host = _host,
                            FileId = res.fileids!.First().ToString(),
                            Size = !fileMeta.isfolder ? fileMeta.size / 1000 : null,
                            ParentFolderId = folderId.ToString(),
                            IsFolder = false,
                            Modified = fileMeta.modified,
                            Created = fileMeta.created
                        });
                    }
                    catch (Exception e)
                    {
                        Console.Error.WriteLine(e.Message);
                        errors.Add($"File upload failed for file: {file.FileName}");
                    }
                }
                _db.SaveChanges();

                TempData["Errors"] = errors;

            }
            return RedirectToAction("Index");
        }

        // Syncs all information from pCloud to App
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> SyncRepo()
        {
            // Clear current db entries for pcloud host
            var currentEntries = _db.UploadedFiles.Where((f) => f.Host == _host && f.Account == _currentUser);
            _db.UploadedFiles.RemoveRange(currentEntries);

            ListFolderRequest req = new(0, true);
            var res = _pClient.Folders.ListFolder(req).Result;

            if (res == null || (res.result != 0 && res.error != null)) throw new Exception("Could not Syncronize: " + res?.error);

            var contents = res.metadata!.contents;

            if (contents != null)
            {
                List<UploadedFile> inserts = new();
                foreach (var meta in contents)
                {
                    inserts.Add(new UploadedFile
                    {
                        FileName = meta.name,
                        Host = _host,
                        FileId = meta.isfolder ? meta.folderid.ToString()! : meta.fileid.ToString()!,
                        ParentFolderId = meta.parentfolderid.ToString(),
                        Size = meta.size / 1000,
                        IsFolder = meta.isfolder,
                        Modified = meta.modified,
                        Created = meta.created
                    });
                }

                await _db.UploadedFiles.AddRangeAsync(inserts);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}