using FiDa.Database;
using FiDa.Models;
using Microsoft.AspNetCore.Mvc;
using pcloud_sdk_csharp.Requests;
using pcloud_sdk_csharp.Controllers;

namespace FiDa.Controllers
{
    public class PCloudController : Controller
    {
        private readonly FiDaDatabase Db = new();
        private readonly IConfiguration _config;
        //private readonly User currentUser = Db.User.Find((u) => u.);
        private readonly Hosts _host = Hosts.pCloud;

        public PCloudController(IConfiguration configuration)
        {
            _config = configuration;
        }

        // 
        // GET: /pcloud/ 
        public ActionResult Index(long? folderId)
        {
            List<FileUpload>? files = Db.UploadedFiles.Where((f) => f.ParentFolderId == (folderId ?? 0)).OrderByDescending((f) => f.IsFolder).ToList();
            return View(files ?? new List<FileUpload>());
        }

        //[HttpPost]
        public ActionResult UploadFile(IFormCollection? form)
        {
            if (form != null && form.Count > 0)
            {
                long folderId = long.Parse(form["Folder"]);
                IFormFile[] files = form.Files.ToArray();

                List<string> errors = new();

                foreach (var file in files)
                {
                    try
                    {
                        UploadFileRequest req = new(folderId, file.FileName, file.OpenReadStream());
                        var res = FileController.UploadFile(req, _config["API_Tokens:PCloud"]).Result;

                        if (res == null || (res.result != 0 && res.error != null)) throw new Exception("PCloud returned an exception: " + res?.error);

                        var fileMeta = res.metadata.First();

                        Db.UploadedFiles.Add(new FileUpload
                        {
                            FileName = file.FileName,
                            Hostname = _host,
                            FileId = res.fileids.First(),
                            Size = !fileMeta.isfolder ? fileMeta.size / 1000 : null,
                            ParentFolderId = folderId,
                            ModificationDate = DateTime.Now
                        });
                    }
                    catch (Exception e)
                    {
                        Console.Error.WriteLine(e.Message);
                        errors.Add($"File upload failed for file: {file.FileName}");
                    }
                }
                Db.SaveChanges();

                TempData["Errors"] = errors;

            }
            return RedirectToAction("Index");
        }

        // Syncs all information from pCloud to App
        public async Task<ActionResult> SyncRepo()
        {
            // Clear current db entries for pcloud host
            var currentEntries = Db.UploadedFiles.Where((f) => f.Hostname == _host);
            Db.UploadedFiles.RemoveRange(currentEntries);

            ListFolderRequest req = new(0, true);
            var res = FolderController.ListFolder(req, _config["API_Tokens:PCloud"]).Result;

            if (res == null || (res.result != 0 && res.error != null)) throw new Exception("Could not Syncronize: " + res?.error);

            var contents = res.metadata.contents;

            if (contents != null)
            {
                List<FileUpload> inserts = new();
                foreach (var meta in contents)
                {
                    inserts.Add(new FileUpload
                    {
                        FileName = meta.name,
                        Hostname = Hosts.pCloud,
                        FileId = meta.isfolder ? (long)meta.folderid! : (long)meta.fileid!,
                        ParentFolderId = meta.parentfolderid,
                        Size = meta.size / 1000,
                        IsFolder = meta.isfolder,
                        ModificationDate = DateTime.Parse(meta.modified),
                        CreationDate = DateTime.Parse(meta.created)
                    });
                }

                await Db.UploadedFiles.AddRangeAsync(inserts);
                await Db.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}