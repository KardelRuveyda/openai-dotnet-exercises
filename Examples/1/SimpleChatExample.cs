using System;
using System.Threading.Tasks;
using OpenAI.Chat;

namespace MyOpenAIProject.Examples
{
    public class SimpleChatExample
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

            // OpenAI ChatClient oluşturun
            ChatClient client = new(model: "gpt-4", apiKey);

            // Sohbet tamamlama işlemini gerçekleştirin
            ChatCompletion completion = client.CompleteChat("Say 'this is a test.'");

            // Sonucu ekrana yazdırın
            Console.WriteLine($"[ASSISTANT]: {completion}");
            Console.ReadLine();
        }

        public static async Task RunAsync()
        {
            // API anahtarını ConfigReader sınıfından alma işlemi
            string apiKey = ConfigReader.ReadApiKeyFromConfig();

            // API anahtarı alınamazsa işlemi sonlandır
            if (string.IsNullOrEmpty(apiKey))
            {
                Console.WriteLine("API key not found in config.json");
                return;
            }

            // OpenAI ChatClient oluşturun
            ChatClient client = new(model: "gpt-4", apiKey);

            // Sohbet tamamlama işlemini gerçekleştirin

            ChatCompletion completion = await client.CompleteChatAsync("Say 'this is a test.'");

            // Sonucu ekrana yazdırın
            Console.WriteLine($"[ASSISTANT]: {completion}");
            Console.ReadLine();
        }
    }
}
