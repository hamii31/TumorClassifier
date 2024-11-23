using TumorClassifier.Models;

namespace TumorClassifier.Services
{
    public class FileService
    {
        public static byte[] ImageToByteArray(string imagePath)
        {
            using (var image = System.Drawing.Image.FromFile(imagePath)) // Load image
            using (var ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png); // Save to memory stream in desired format
                return ms.ToArray(); // Return byte array
            }
        }
    }
}
