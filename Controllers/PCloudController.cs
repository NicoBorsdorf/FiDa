using FiDa.Database;
using Microsoft.AspNetCore.Mvc;
using pcloud_sdk_csharp.Requests;
using pcloud_sdk_csharp.Controllers;
using Microsoft.AspNetCore.Authorization;
using FiDa.DatabaseModels;
using FiDa.ViewModels;

namespace FiDa.Controllers
{
    public class PCloudController : Controller
    {
        private readonly FiDaDatabase _db = new();
        private readonly Account _currentUser;
        private readonly UserHost _host;

        public PCloudController()
        {
            if (User.Identity?.Name == null) throw new Exception("No User provided.");
            _currentUser = _db.Account.First((a) => a.Username == User.Identity.Name);

            if (_currentUser == null) throw new Exception("No User found in Database");

            _host = _currentUser.ConfiguredHosts.First((h) => h.Host == Hosts.PCloud);
            if (_host == null || _host?.ApiKey == null) throw new Exception("No or faulty host configuration found for pCloud");
        }

        // 
        // GET: /pcloud/ 
        [Authorize]
        public ActionResult Index(long? folderId)
        {
            var _files = _db.UploadedFiles.Where((f) => f.Account == _currentUser && f.Host == Hosts.PCloud);
            FileViewModel model = new();

            model.RootFiles = _files.Where((f) => f.ParentFolderId == (folderId ?? 0)).OrderByDescending((f) => f.IsFolder).ToList();
            model.Folders = _files.Where((f) => f.IsFolder).ToList();

            return View(model);
        }

        //[HttpPost]
        [Authorize]
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
                        var res = FileController.UploadFile(req, _currentUser.ConfiguredHosts.First((h) => h.Host == Hosts.PCloud).ApiKey).Result;

                        if (res == null || (res.result != 0 && res.error != null)) throw new Exception("PCloud returned an exception: " + res?.error);

                        var fileMeta = res.metadata.First();

                        _db.UploadedFiles.Add(new UploadedFile
                        {
                            FileName = file.FileName,
                            Host = _host.Host,
                            FileId = res.fileids.First(),
                            Size = !fileMeta.isfolder ? fileMeta.size / 1000 : null,
                            ParentFolderId = folderId,
                            IsFolder = false,
                            Modified = DateTime.Parse(fileMeta.modified),
                            Created = DateTime.Parse(fileMeta.created)
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
        [Authorize]
        public async Task<ActionResult> SyncRepo()
        {
            // Clear current db entries for pcloud host
            var currentEntries = _db.UploadedFiles.Where((f) => f.Host == _host.Host && f.Account == _currentUser);
            _db.UploadedFiles.RemoveRange(currentEntries);

            ListFolderRequest req = new(0, true);
            var res = FolderController.ListFolder(req, _host.ApiKey).Result;

            if (res == null || (res.result != 0 && res.error != null)) throw new Exception("Could not Syncronize: " + res?.error);

            var contents = res.metadata.contents;

            if (contents != null)
            {
                List<UploadedFile> inserts = new();
                foreach (var meta in contents)
                {
                    inserts.Add(new UploadedFile
                    {
                        FileName = meta.name,
                        Host = _host.Host,
                        FileId = meta.isfolder ? (long)meta.folderid! : (long)meta.fileid!,
                        ParentFolderId = meta.parentfolderid,
                        Size = meta.size / 1000,
                        IsFolder = meta.isfolder,
                        Modified = DateTime.Parse(meta.modified),
                        Created = DateTime.Parse(meta.created)
                    });
                }

                await _db.UploadedFiles.AddRangeAsync(inserts);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}