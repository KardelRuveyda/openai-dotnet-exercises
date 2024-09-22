using OpenAI.Images;

namespace MyOpenAIProject.Examples
{
    public class ImageGenerateExample
    {
        public static void Run()
        {
            // API anahtarını ConfigReader sınıfından alma işlemi
            string apiKey = ConfigReader.ReadApiKeyFromConfig();

            // API anahtarı alınamazsa işlemi sonlandır
            if (string.IsNullOrEmpty(apiKey))
            {
                Console.WriteLine("API key not found in config.json");
                return;
            }

            ImageClient client = new(model: "dall-e-3", apiKey);
            string prompt = "Yeni mezun erkek ve kadın yazılımcılar olsun. İkisinin de kafası karışık olsun hangi alana yöneleceğim diye düşünsün.";

            ImageGenerationOptions options = new()
            {
                Quality = GeneratedImageQuality.High,
                Size = GeneratedImageSize.W1792xH1024,
                Style = GeneratedImageStyle.Vivid,
                ResponseFormat = GeneratedImageFormat.Bytes
            };
            Console.WriteLine("Fotoğraf oluşturuluyor..");
            GeneratedImage image = client.GenerateImage(prompt, options);
            BinaryData bytes = image.ImageBytes;

            using FileStream stream = File.OpenWrite($"{Guid.NewGuid()}.png");
            bytes.ToStream().CopyTo(stream);

            Console.WriteLine("Fotoğraf oluşturuldu...");

        }

    }
}
