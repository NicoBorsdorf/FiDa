
using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace FiDa.Controllers
{
    public class OneDriveController : Controller
    {
        // 
        // GET: /PCloud/ 

        public ActionResult Index()
        {
            return View();
        }

        // 
        // GET: /HelloWorld/Welcome/ 

        public string Welcome()
        {
            return "This is the Welcome action method...";
        }
    }
}