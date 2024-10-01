using OpenAI.Chat;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyOpenAIProject.Examples
{
    public class StreamingChatExample
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
            CollectionResult<StreamingChatCompletionUpdate> updates
                = client.CompleteChatStreaming("Say 'this is a test.'");
            // Sonucu ekrana yazdırın
            Console.WriteLine($"[ASSISTANT]:");
            foreach (StreamingChatCompletionUpdate update in updates)
            {
                foreach (ChatMessageContentPart updatePart in update.ContentUpdate)
                {
                    Console.Write(updatePart);
                }
            }
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

            AsyncCollectionResult<StreamingChatCompletionUpdate> updates
                = client.CompleteChatStreamingAsync("Say 'this is a test.'");

            Console.WriteLine($"[ASSISTANT]:");
            await foreach (StreamingChatCompletionUpdate update in updates)
            {
                foreach (ChatMessageContentPart updatePart in update.ContentUpdate)
                {
                    Console.Write(updatePart.Text);
                }
            }
            Console.ReadLine();
        }


    }
}
