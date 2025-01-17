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

            Console.WriteLine("-----API KEY OKUNDU-----");
            // API anahtarı alınamazsa işlemi sonlandır
            if (string.IsNullOrEmpty(apiKey))
            {
                Console.WriteLine("API key not found in config.json");
                return;
            }

            // OpenAI ChatClient oluşturun
            ChatClient client = new(model: "gpt-4o", apiKey);

            // Sohbet tamamlama işlemini gerçekleştirin
            ChatCompletion completion = client.CompleteChat("“Bu bir test” deyin.'");

            // Sonucu ekrana yazdırın
            Console.WriteLine($"[ASSISTANT]: {completion.Content[0].Text}");
            Console.ReadLine();
        }

        public static async Task RunAsync()
        {
            // API anahtarını ConfigReader sınıfından alma işlemi
            string apiKey = ConfigReader.ReadApiKeyFromConfig();

            Console.WriteLine("-----API KEY OKUNDU-----");

            // API anahtarı alınamazsa işlemi sonlandır
            if (string.IsNullOrEmpty(apiKey))
            {
                Console.WriteLine("API key not found in config.json");
                return;
            }

            ChatClient client = new(model: "gpt-4o", apiKey);

            ChatCompletion completion = await client.CompleteChatAsync("“Bu bir test” deyin.'");

            Console.WriteLine($"[ASSISTANT]: {completion.Content[0].Text}");
        }
    }
}
