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
        public ActionResult UploadFile(IFormFile[] files)
        {
            Console.WriteLine("length " + files.Length);
            if (files != null && files.Length > 0)
            {
                foreach (var file in files)
                {
                    try
                    {
                        throw new Exception("sdafgh");
                        StreamReader _file = new(file.OpenReadStream());
                        UploadFileRequest req = new(0, file.FileName, _file);
                        var res = FileController.UploadFile(req, "64qbZMEJjKgdvE57ZUI7t7kZCApm81pUrChgxtJQwE7BBHjgQbJV").Result;
                        Console.WriteLine(res);
                        ViewBag.Message = "File Uploaded Successfully!!";
                    }
                    catch
                    {
                        Console.WriteLine("filename " + file.FileName);
                        ViewBag.Message += $"File upload failed for file: {file.FileName} <br />";
                    }
                }
            }
            else
            {
                ViewBag.Message = "Please select a file.";
            }
            return PartialView("_UploadFile");
        }


        //
        // GET: /pcloud/{fileId}
        public ActionResult FileView(int? fileId)
        {
            if (fileId == null)
            {
                return BadRequest("Bad Request");
            }

            FileUpload? file = db.UploadedFiles.Find(fileId);
            if (file == null)
            {
                return NotFound();
            }

            return View(file);
        }
    }
}