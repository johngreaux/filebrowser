using System.Collections.Generic;
using System.IO;
using System.Web.Http;
using FileBrowser.Models;
using System.Net.Http;
using System.Net;
using System.Web;
using System;

namespace FileBrowser.Controllers
{
    public class FileController : ApiController
    {
       /***************************************************************
       * GetFiles
       * path:    GET /api/file/?path&filter
       * author:  john greaux
       * date:    05/17/2017
       * desc:    Builds a list of files, folders, and statistics for a
       *          given path and search filter
       * param:   path - the directory path to enumerate folder and files from
       *          filter - optional directory search filter
       * returns: JSON result - List<FileViewModel()
       **************************************************************/
        [HttpGet]
        public IHttpActionResult GetFiles(string path, string filter)
        {
            // Default values
            filter = filter == null ? "*.*" : filter;
            var root = HttpContext.Current.Server.MapPath("~\\");
            var model = new BrowserViewModel();
            var totalSize = 0.00M;
            model.TotalFiles = 0;
            model.TotalFolders = 0;
            model.Files = new List<FileViewModel>();

            try
            {
                // Get all folders for the given path
                foreach (var d in Directory.EnumerateDirectories(HttpContext.Current.Server.MapPath(path), filter))
                {
                    var info = new DirectoryInfo(d);
                    model.TotalFolders++;

                    // Add folder to FileViewModel list
                    model.Files.Add(new FileViewModel()
                    {
                        Path = info.FullName.Replace(root, "~\\"),
                        Name = info.Name,
                        LastModified = info.LastWriteTime.ToShortDateString(),
                        Type = "File Folder",
                    });
                }

                // Get all files for the given path
                foreach (var f in Directory.EnumerateFiles(HttpContext.Current.Server.MapPath(path), filter))
                {
                    var info = new FileInfo(f);
                    model.TotalFiles++;
                    totalSize += decimal.Divide(info.Length, 1024);

                    // Add file to FileViewModel list
                    model.Files.Add(new FileViewModel()
                    {
                        Path = info.FullName.Replace(root, "~\\"),
                        Name = info.Name,
                        LastModified = info.LastWriteTime.ToShortDateString(),
                        Type = info.Extension,
                        Size = decimal.Divide(info.Length, 1024).ToString("#.##")
                    });
                }

                // Format total size decimal
                model.TotalSize = totalSize.ToString("#.##");
                return Json(new { success = true, model });
            }
            catch (Exception e)
            {
                return Json(new { success = false, exception = e.Message });
            }
           
        }


        /***************************************************************
       * Download
       * path:    GET /api/file/?path&name
       * author:  john greaux
       * date:    05/18/2017
       * desc:    Returns a file stream based on the given path and file 
       *          name
       * param:   path - the path to download from
       *          name - the name of the file
       * returns: HttpResponseMessage
       **************************************************************/
        [HttpGet]
        public HttpResponseMessage Download(string path, string name)
        {
            // Open file stream using given path
            var stream = new FileStream(HttpContext.Current.Server.MapPath(path), FileMode.Open);

            // Build HttpResponseMessage
            var file = new HttpResponseMessage(HttpStatusCode.OK);
            file.Content = new StreamContent(stream);
            file.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
            file.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            file.Content.Headers.ContentDisposition.FileName = name;

            return file;
        }

          /***************************************************************
          * Copy
          * path:    GET /api/file/?currentPath&newPath&fileName&isMove
          * author:  john greaux
          * date:    05/19/2017
          * desc:    Copies a file based on the given parameters
          * param:   currentPath - the location the file currently resides at
          *          newPath - the location that the file will go to
          *          fileName - the file name
          * returns: JSON - success true or false
          **************************************************************/
        [HttpGet]
        public IHttpActionResult Copy(string currentPath, string newPath, string fileName)
        {
            try
            {
                // Copy the file to the new locations
                File.Copy(HttpContext.Current.Server.MapPath(currentPath), Path.Combine(HttpContext.Current.Server.MapPath(newPath), fileName));
  
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, exception = e.Message });
            }
        }

        /***************************************************************
          * Move
          * path:    GET /api/file/?currentPath&newPath&fileName&isMove
          * author:  john greaux
          * date:    05/19/2017
          * desc:    Moves a file based on the given parameters
          * param:   currentPath - the location the file currently resides at
          *          newPath - the location that the file will go to
          *          fileName - the file name
          * returns: JSON - success true or false
          **************************************************************/
        [HttpGet]
        public IHttpActionResult Move(string currentPath, string newPath, string fileName)
        {
            try
            {
                // Move the file to the new location
                File.Move(HttpContext.Current.Server.MapPath(currentPath), Path.Combine(HttpContext.Current.Server.MapPath(newPath), fileName));
              
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, exception = e.Message });
            }
        }

        /***************************************************************
          * Upload
          * path:    POST /api/file/
          * author:  john greaux
          * date:    05/18/2017
          * desc:    Uploads a file to the server
          * param:   N/A - All file data is passed through HttpContext
          * returns: JSON - success true or false
          **************************************************************/
        [HttpPost]
        public IHttpActionResult Upload()
        {
            try
            {
                // Get file and path info from HttpContext
                var file = HttpContext.Current.Request.Files["File"];
                var path = HttpContext.Current.Request.Form["Path"];

                // Save file to path
                file.SaveAs(Path.Combine(HttpContext.Current.Server.MapPath(path), file.FileName));
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, exception = e.Message });
            }

        }

        /***************************************************************
          * Delete
          * path:    DELETE /api/file/
          * author:  john greaux
          * date:    05/18/2017
          * desc:    Removes a file from the server
          * param:   N/A - path data is passed through HttpContext
          * returns: JSON - success true or false
          **************************************************************/
        [HttpDelete]
        public IHttpActionResult Delete()
        {
            try
            {
                // Remove file from server based on HttpContext request variable Path
                File.Delete(HttpContext.Current.Server.MapPath(HttpContext.Current.Request.Form["Path"]));
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, exception = e.Message});
            }
        }
    }
}