using FiDa.Database;
using FiDa.Models;
using Microsoft.AspNetCore.Mvc;
//using System.Web.Mvc;

namespace FiDa.Controllers
{
    public class PCloudController : Controller
    {
        private readonly FiDaDatabase db = new();

        // 
        // GET: /pcloud/ 
        public ActionResult Index(IFormFile? file)
        {
            List<FileUpload> files = db.UploadedFiles.ToList();
            Console.WriteLine(files);

            if (file != null)
            {
                try
                {
                    if (file.Length > 0)
                    {

                    }
                    ViewBag.Message = "File Uploaded Successfully!!";
                }
                catch
                {
                    ViewBag.Message = "File upload failed!!";
                }
            }

            return View(files);
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