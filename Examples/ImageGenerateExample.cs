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
            string prompt = "2024 yılında Almanya'da düzenlenecek olan Euro 2024 için Türkiye millî futbol takımını temsil eden etkileyici bir tanıtım görseli oluşturun. Türkiye'nin futbol tutkusunu, millî takımın coşkusunu ve zafer için verdiği mücadeleyi yansıtan bir görüntü hayal edin. Tribünlerde coşkuyla destek veren taraftarlar, sahadaki mücadele dolu anlar ve millî takımın başarısını simgeleyen sembollerle dolu bir kompozisyon oluşturun. Görüntü, güçlü bir duygu uyandırmalı ve Türkiye'nin Euro 2024'teki başarısına olan inancı ve heyecanı yansıtmalı.";

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
