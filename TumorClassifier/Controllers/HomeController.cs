using Microsoft.AspNetCore.Mvc;
using nClam;
using System.Diagnostics;
using TumorClassifier.Models;
using static TumorClassifier.AppConstants.Size;
using static TumorClassifier.AppConstants.ClamClientConstants;

namespace TumorClassifier.Controllers
{
    public class HomeController : Controller
    {
        private readonly string[] allowedExtensions = { ".png" };
        private readonly ClamClient clamClient = new ClamClient(server, port);
        public IActionResult Index()
        {
            return View(new FileViewModel());
        }

        /// <summary>
        /// Restricts file uploads to a maximum size defined by fileMaxSize (2 MB).
        /// Only allows specific file extensions (.png).
        /// Checks for magic numbers.
        /// Creates a unique file name using GUID before saving the file.
        /// Returns user-friendly error messages for validation failures.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UploadFile(FileViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (model.File != null)
                    {

                        // Check file size
                        if (model.File.Length > fileMaxSize)
                        {
                            ModelState.AddModelError("File", "File size must be less than 2 MB.");
                            return View("Index", model);
                        }

                        // Check file extension
                        string fileExtension = Path.GetExtension(model.File.FileName).ToLowerInvariant();
                        if (!allowedExtensions.Contains(fileExtension))
                        {
                            ModelState.AddModelError("File", "Please upload a valid PNG file.");
                            return View("Index", model);
                        }

                        // Check MIME type
                        string contentType = model.File.ContentType;
                        if (contentType != "image/png")
                        {
                            ModelState.AddModelError("File", "Please upload a valid PNG file.");
                            return View("Index", model);
                        }

                        // Read the file content to ensure it's a valid image.
                        using (var memoryStream = new MemoryStream())
                        {
                            await model.File.CopyToAsync(memoryStream);
                            byte[] fileContent = memoryStream.ToArray();

                            // Validate the content header (magic number) for PNG
                            if (fileContent.Length < 8 ||
                                fileContent[0] != 0x89 ||
                                fileContent[1] != 0x50 ||
                                fileContent[2] != 0x4E ||
                                fileContent[3] != 0x47 ||
                                fileContent[4] != 0x0D ||
                                fileContent[5] != 0x0A ||
                                fileContent[6] != 0x1A ||
                                fileContent[7] != 0x0A)
                            {
                                ModelState.AddModelError("File", "Uploaded file is not a valid PNG image.");
                                return View("Index", model);
                            }

                           
                            // Rename file to a unique name
                            Guid fileId = Guid.NewGuid();
                            string day = DateTime.UtcNow.Day.ToString();
                            string month = DateTime.UtcNow.Month.ToString();
                            string year = DateTime.UtcNow.Year.ToString();
                            string hour = DateTime.UtcNow.Hour.ToString();
                            string minutes = DateTime.UtcNow.Minute.ToString();


                            string dateSalt = "=" + day + "_" + month + "_" + year + "-" + hour + "_" + minutes;

                            string newFileName = Path.Combine(fileId.ToString(), dateSalt /*Maybe add IP of user too */).Replace(@"\", string.Empty);
                            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadedFiles");

                            if (!Directory.Exists(folderPath))
                            {
                                Directory.CreateDirectory(folderPath);
                            }

                            string filePath = Path.Combine(folderPath, newFileName);
                            using (FileStream stream = new FileStream(filePath, FileMode.Create))
                            {
                                await memoryStream.CopyToAsync(stream);
                            }

                            ViewData["Message"] = $"File uploaded successfully: {newFileName}";
                        }
                    }
                    else
                        ViewData["Message"] = "Upload a file.";
                }
                catch (Exception ex)
                {
                    ViewData["Message"] = $"File upload failed: {ex.Message}";
                }
            }

            return View("Index", model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
