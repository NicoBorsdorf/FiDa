using FiDa.Database;
using FiDa.Models;
using Microsoft.AspNetCore.Mvc;
using pcloud_sdk_csharp.Controllers;
using pcloud_sdk_csharp.Requests;

namespace FiDa.Controllers
{
    public class PCloudController : Controller
    {
        private readonly FiDaDatabase db = new();

        // 
        // GET: /pcloud/ 
        public ActionResult Index()
        {
            List<FileUpload> files = db.UploadedFiles.ToList();
            return View(files);
        }

        [HttpPost]
        public ActionResult UploadFile(IFormCollection form)
        {
            int folderId = int.Parse(form["Folder"]);
            IFormFile[] files = form.Files.ToArray();

            foreach (var file in files)
            {
                Console.WriteLine("file " + file.FileName);
                //try
                //{
                StreamReader _file = new(file.OpenReadStream());
                UploadFileRequest req = new(folderId, file.FileName, _file);
                var res = FileController.UploadFile(req, "64qbZMEJjKgdvE57ZUI7t7kZCApm81pUrChgxtJQwE7BBHjgQbJV");
                res.Wait();

                Console.WriteLine(res.Result);
                ViewBag.Message = "File Uploaded Successfully!!";
                //}
                //catch (Exception e)
                //{
                //    Console.WriteLine($"Exception for {file.FileName} occured: {e.Message}");
                //    ViewBag.Message += $"File upload failed for file: {file.FileName} <br />";
                //}
            }

            return View("Index");
        }
    }
}