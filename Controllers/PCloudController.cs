
using FiDa.Models;
using System.Net;
using System.Web.Mvc;

namespace FiDa.Controllers
{
    public class PCloudController : Controller
    {
        private UploadedFileDBContext db = new UploadedFileDBContext();
        // 
        // GET: /pcloud/ 

        public ActionResult Index()
        {
            return View(db.UploadedFiles.ToList());
        }

        //
        // GET: /pcloud/{fileId}
        public ActionResult FileView(int? fileId)
        {
            if (fileId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            FileUpload file = db.UploadedFiles.Find(fileId);
            if (file == null)
            {
                return HttpNotFound();
            }

            return View(file);
        }
    }
}