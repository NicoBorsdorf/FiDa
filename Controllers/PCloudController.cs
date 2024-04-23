using FiDa.Database;
using FiDa.Models;
using Microsoft.AspNetCore.Mvc;
using pcloud_sdk_csharp.Requests;
using pcloud_sdk_csharp.Controllers;
using System.Text.Json;

namespace FiDa.Controllers
{
    public class PCloudController : Controller
    {
        private readonly FiDaDatabase Db = new();
        private readonly IConfiguration _config;

        public PCloudController(IConfiguration configuration)
        {
            _config = configuration;
        }

        // 
        // GET: /pcloud/ 
        public ActionResult Index()
        {
            List<FileUpload> files = Db.UploadedFiles.ToList();
            return View(files);
        }

        //[HttpPost]
        public ActionResult UploadFile(IFormCollection form)
        {
            Console.WriteLine("folder " + form["Folder"]);
            int folderId = int.Parse(form["Folder"]);
            IFormFile[] files = form.Files.ToArray();

            foreach (var file in files)
            {
                try
                {
                    UploadFileRequest req = new(folderId, file.FileName, file.OpenReadStream(), null);

                    var res = FileController.UploadFile(req, _config["API_Tokens:PCloud"]).Result;

                    Console.WriteLine(JsonSerializer.Serialize(res));

                    ViewBag.Message = "File Uploaded Successfully!!";
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.Message);
                    //Console.WriteLine($"Exception for {file.FileName} occured: {e.Message}");
                    //ViewBag.Message += $"File upload failed for file: {file.FileName} <br />";
                }
            }

            return View("Index");
        }
    }
}