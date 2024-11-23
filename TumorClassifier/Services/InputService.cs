namespace TumorClassifier.Services
{
    public class InputService
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
