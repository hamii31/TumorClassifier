using Microsoft.AspNetCore.Mvc;
using TumorClassifier.Models;

namespace TumorClassifier.Controllers
{
    public class FileUploadController : Controller
    {
        public IActionResult Index()
        {
            return View(new FileViewModel());
        }
        [HttpPost]
        public async Task<IActionResult> UploadFile(FileViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (model.File != null)
                    {
                        // Check file extension
                        var fileExtension = Path.GetExtension(model.File.FileName).ToLowerInvariant();
                        // Check MIME type
                        var isValidFileType = model.File.ContentType == "image/png" || fileExtension == ".png";

                        if (!isValidFileType)
                        {
                            ModelState.AddModelError("File", "Please upload a file of type PNG.");
                            return View("Index", model);
                        }

                        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadedFiles");

                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }

                        var filePath = Path.Combine(folderPath, model.File.FileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.File.CopyToAsync(stream);
                        }

                        ViewData["Message"] = $"File uploaded successfully: {model.File.FileName}";
                    }
                }
                catch (Exception ex)
                {
                    ViewData["Message"] = $"File upload failed: {ex.Message}";
                }
            }

            return View("Index", model);
        }
    }
}
