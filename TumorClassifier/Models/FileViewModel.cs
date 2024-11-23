using System.ComponentModel.DataAnnotations;

namespace TumorClassifier.Models
{
    public class FileViewModel
    {
        [Required(ErrorMessage = "Please select a file.")]
        public IFormFile File { get; set; }
    }
}
