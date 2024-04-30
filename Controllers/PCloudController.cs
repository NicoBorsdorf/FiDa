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
        private readonly string _token = Db.User.

        public PCloudController(IConfiguration configuration)
        {
            _config = configuration;
        }

        // 
        // GET: /pcloud/ 
        public ActionResult Index()
        {
            List<FileUpload> files = Db.UploadedFiles.ToList();
            Console.WriteLine(files);
            return View(files ?? new List<FileUpload>());
        }

        //[HttpPost]
        public ActionResult UploadFile(IFormCollection? form)
        {
            if (form != null && form.Count > 0)
            {
                long folderId = long.Parse(form["Folder"]);
                IFormFile[] files = form.Files.ToArray();

                foreach (var file in files)
                {
                    try
                    {
                        UploadFileRequest req = new(folderId, file.FileName, file.OpenReadStream());
                        var res = FileController.UploadFile(req, _config["API_Tokens:PCloud"]).Result;

                        if (res == null || (res.result != 0 && res.error != null)) throw new Exception("PCloud returned an exception: " + res?.error);

                        Db.UploadedFiles.Add(new FileUpload
                        {
                            FileName = file.FileName,
                            Host = "pCloud",
                            Size = (res.metadata?.First().size ?? 0) / 1000,
                            ParentFolderId = folderId,
                            ModificationDate = DateTime.Now,
                            CreatedDate = DateTime.Now,
                        });

                        ViewBag.Message = "File Uploaded Successfully!!";
                    }
                    catch (Exception e)
                    {
                        Console.Error.WriteLine(e.Message);
                        ViewBag.Message += $"File upload failed for file: {file.FileName} <br />";
                    }

                    Db.SaveChanges();

                }
                return View("Index");
            }
            return PartialView("_UploadFile");
        }
    }
}