using FiDa.Models;
using Microsoft.AspNetCore.Mvc;
//using System.Web.Mvc;

namespace FiDa.Controllers
{
    public class PCloudController : Controller
    {
        // 
        // GET: /pcloud/ 

        public ActionResult Index()
        {
            return View(/*db.UploadedFiles.ToList()*/);
        }

        //
        // GET: /pcloud/{fileId}
        public ActionResult FileView(int? fileId)
        {
            if (fileId == null)
            {
                return BadRequest("Bad Request");
            }

            FileUpload file = null;//db.UploadedFiles.Find(fileId);
            if (file == null)
            {
                return NotFound();
            }

            return View(file);
        }
    }
}