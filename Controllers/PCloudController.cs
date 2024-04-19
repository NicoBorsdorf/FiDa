using FiDa.Database;
using FiDa.Models;
using Microsoft.AspNetCore.Mvc;
using pcloud_sdk_csharp.Requests;
using pcloud_sdk_csharp.Responses;
using System.Text.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace FiDa.Controllers
{
    public class PCloudController : Controller
    {
        private readonly FiDaDatabase Db = new();
        private readonly IConfiguration _Config;

        public PCloudController(IConfiguration configuration)
        {
            _Config = configuration;
        }

        // 
        // GET: /pcloud/ 
        public ActionResult Index()
        {
            List<FileUpload> files = Db.UploadedFiles.ToList();
            return View(files);
        }

        [HttpPost]
        public ActionResult UploadFile(IFormCollection form)
        {
            int folderId = int.Parse(form["Folder"]);
            IFormFile[] files = form.Files.ToArray();

            foreach (var file in files)
            {
                try
                {
                    Stream _file = file.OpenReadStream();
                    Console.WriteLine("folderid " + folderId);
                    Console.WriteLine("_file.Length " + _file.Length);
                    Console.WriteLine("file.Name " + file.FileName);
                    UploadFileRequest req = new(folderId, file.FileName, _file);
                    var res = FileController.UploadFile(req, _Config["API_Tokens:PCloud"]).Result;

                    Console.WriteLine(res.ToString());
                    Console.WriteLine(res.fileids.Count());
                    Console.WriteLine(res.metadata.Count());
                    ViewBag.Message = "File Uploaded Successfully!!";
                }
                catch (Exception e)
                {
                    //Console.WriteLine($"Exception for {file.FileName} occured: {e.Message}");
                    //ViewBag.Message += $"File upload failed for file: {file.FileName} <br />";
                }
            }

            return View("Index");
        }

        public class FileController
        {
            private static readonly string baseURL = "https://eapi.pcloud.com/";
            private static readonly HttpClient client = new();

            public async static Task<UploadedFile?> UploadFile(UploadFileRequest req, string token)
            {
                var formData = new MultipartFormDataContent();
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                formData.Add(new StringContent(req.FolderId.ToString()), "folderid");
                formData.Add(new ByteArrayContent(req.UploadFile), "file");
                formData.Add(new StringContent(req.FileName), "filename");

                Console.WriteLine(formData.ToString());
                Console.WriteLine("formdata " + JsonSerializer.Serialize(formData));

                HttpResponseMessage response = await client.PostAsync(baseURL + "uploadfile", formData);

                return JsonSerializer.Deserialize<UploadedFile>(await response.Content.ReadAsStringAsync());
            }
        }
    }
}