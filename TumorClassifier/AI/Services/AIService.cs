using BreastCancerImageAssessmentMLM;
using static TumorClassifier.Services.InputService;

namespace TumorClassifier.AI.Services
{
    public class AIService
    {
        public static string Consume()
        {
            string imagePath = @"C:../../../IO/image_1.png";

            string result = ImageCheck(imagePath).ToString()!;

            return result;
        }
        public static string ImageCheck(string imagePath)
        {
            try
            {
                byte[] imageBytes = ImageToByteArray(imagePath);

                var sampleData = new BCImageAssessmentModel.ModelInput()
                {
                    ImageSource = imageBytes
                };

                var result = BCImageAssessmentModel.Predict(sampleData);

                if (result.PredictedLabel == "1")
                {
                    return "Malignant";
                }
                else
                {
                    return "Benign";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return null!;

        }

    }
}
