using Dropbox.Api;
using Dropbox.Api.Files;
using FiDa.Database;
using FiDa.DatabaseModels;
using FiDa.Lib;
using FiDa.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace FiDa.Controllers
{

    [Route("dropbox/[action]")]
    public class DropboxController : Controller
    {
        private readonly Account _currentUser;
        private readonly UserHost _host;
        private readonly FiDaDatabase _db = new(options: new());
        private readonly DropboxClient _dropboxClient;
        private readonly ILogger _logger;

        public DropboxController(IHttpContextAccessor httpContextAccessor, ILogger<DropboxController> logger)
        {
            _logger = logger;
            _logger.LogInformation("New instance DropboxController");

            var uName = httpContextAccessor.HttpContext?.User.Identity?.Name ?? throw new ArgumentNullException("Username");
            _currentUser = _db.Account.Include(a => a.ConfiguredHosts).FirstOrDefault(a => a.Username == uName) ?? throw new Exception($"No Account for {uName} found on database");

            _host = _currentUser.ConfiguredHosts.FirstOrDefault((h) => h.Host == Hosts.Dropbox)!;
            if (null == _host)
            {
                _logger.LogError("DropboxController - could not find host configuration for user: {user}", _currentUser.Username);
                throw new Exception("No Dropbox host configured for user.");
            }

            _dropboxClient = new(oauth2AccessToken: _host.ApiKey, oauth2RefreshToken: _host.RefreshToken, oauth2AccessTokenExpiresAt: (DateTime)_host.TokenExpiration!, appKey: _host.AppKey, appSecret: _host.AppSecret);
            _dropboxClient.RefreshAccessToken(null).Wait();
        }

        [Authorize]
        public ActionResult Index(string? id)
        {
            _logger.LogInformation("DropboxController Index - folderId: {folderId}", id);

            var _files = _db.UploadedFiles.Where((f) => f.Account == _currentUser && f.Host == _host);
            var fileViewModel = new FileViewModel
            {
                RootFiles = _files.Where((f) => f.ParentFolderId == (id ?? "")).OrderByDescending((f) => f.IsFolder).ToList(),
                Folders = _files.Where((f) => f.IsFolder).ToList()
            };
            var model = new BaseViewModel
            {
                Account = _currentUser,
                FileViewModel = fileViewModel
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> UploadFile(IFormCollection? form)
        {
            _logger.LogInformation("DropboxController UploadFile - formCount: {count}", form?.Count);

            if (form != null && form.Count > 0)
            {
                string folder = (string?)form["Folder"] ?? throw new ArgumentNullException("folder");
                IFormFile[] files = form.Files.ToArray();

                List<string> errors = new();

                foreach (var file in files)
                {
                    try
                    {
                        var args = new UploadArg(path: string.IsNullOrEmpty(folder) ? $"/{file.FileName}" : folder, autorename: true);
                        var res = await _dropboxClient.Files.UploadAsync(args, file.OpenReadStream()) ?? throw new Exception("Dropbox upload file returned null.");

                        var newFile = new UploadedFile
                        {
                            FileName = res.Name,
                            Account = _currentUser,
                            AccountId = _currentUser.Id,
                            Created = res.IsFile ? res.AsFile.FileLockInfo?.Created ?? DateTime.Now : DateTime.Now,
                            FileId = res.IsFile ? res.AsFile.Id : res.AsFolder.Id,
                            Host = _host,
                            HostId = _host.Id,
                            IsFolder = res.IsFolder,
                            Modified = res.IsFile ? res.AsFile.ServerModified : DateTime.Now,
                            ParentFolderId = !string.IsNullOrEmpty(Regex.Match(res.PathLower, @"\/([^\/]*)\/[^\/]*$").Groups[1].Value) ? _db.UploadedFiles.Where(f => f.Account == _currentUser && f.Host == _host && f.FileName.ToLower().Trim() == Regex.Match(res.PathLower, @"\/([^\/]*)\/[^\/]*$").Groups[1].Value).First().FileId : string.Empty,
                            Size = res.IsFile ? res.AsFile.Size : null
                        };

                        _db.Account.Update(_currentUser);
                        _db.UserHost.Update(_host);
                        await _db.UploadedFiles.AddAsync(newFile);

                        await _db.SaveChangesAsync();
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "An error occurred while uploading a file.");
                        errors.Add($"File upload failed for file {file.FileName} with error {e.Message}");
                    }
                }
                TempData["Errors"] = errors;
            }
            return RedirectToAction("Index");
        }

        [Authorize]
        public async Task<ActionResult> SyncRepo()
        {
            _logger.LogInformation("DropboxController - SyncRepo");

            var existingFiles = _db.UploadedFiles.Where(f => f.Account == _currentUser && f.Host == _host);
            if (existingFiles.Any())
            {
                _db.UploadedFiles.RemoveRange(existingFiles);
                await _db.SaveChangesAsync();
                _host.Files.Clear();
            }

            var dropBoxContent = await _dropboxClient.Files.ListFolderAsync(new ListFolderArg(path: "", recursive: true)) ?? throw new Exception("Dropbox list folder returned null.");

            _logger.LogInformation("Dropbox retuned success with {files} found files.", dropBoxContent.Entries.Count);

            foreach (var e in dropBoxContent.Entries)
            {
                _host.Files.Add(new UploadedFile
                {
                    FileName = e.Name,
                    Account = _currentUser,
                    AccountId = _currentUser.Id,
                    Created = e.IsFile ? e.AsFile.FileLockInfo?.Created ?? DateTime.Now : DateTime.Now,
                    FileId = e.IsFile ? e.AsFile.Id : e.AsFolder.Id,
                    Host = _host,
                    HostId = _host.Id,
                    IsFolder = e.IsFolder,
                    Modified = e.IsFile ? e.AsFile.ServerModified : DateTime.Now,
                    ParentFolderId = !string.IsNullOrEmpty(Regex.Match(e.PathLower, @"\/([^\/]*)\/[^\/]*$").Groups[1].Value) ? dropBoxContent.Entries.Where(f => f.Name.ToLower().Trim() == Regex.Match(e.PathLower, @"\/([^\/]*)\/[^\/]*$").Groups[1].Value).First().AsFolder.Id : string.Empty,
                    Size = e.IsFile ? e.AsFile.Size : null
                });
            }

            _db.Account.Update(_currentUser);
            _db.UserHost.Update(_host);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [Authorize]
        public async Task<ActionResult> DeleteHost() => await Utils.DeleteHost(_host, _logger) ? RedirectToActionPermanent("Index", "Dashboard") : RedirectToPage("Error");
    }
}