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

            Console.WriteLine("-----API KEY OKUNDU-----");

            // API anahtarı alınamazsa işlemi sonlandır
            if (string.IsNullOrEmpty(apiKey))
            {
                Console.WriteLine("API key not found in config.json");
                return;
            }

            ChatClient client = new(model: "gpt-4o", apiKey);

            CollectionResult<StreamingChatCompletionUpdate> completionUpdates = 
                client.CompleteChatStreaming("Herkes hoş geldi ben Kardel Rüveyda Çetin demeni istiyorum.");

            Console.Write($"[ASSISTANT]: ");
            foreach (StreamingChatCompletionUpdate completionUpdate in completionUpdates)
            {
                if (completionUpdate.ContentUpdate.Count > 0)
                {
                    Console.Write(completionUpdate.ContentUpdate[0].Text);
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
            ChatClient client = new(model: "gpt-4o", apiKey);

            AsyncCollectionResult<StreamingChatCompletionUpdate> completionUpdates =
                client.CompleteChatStreamingAsync("Herkes hoş geldi ben Kardel Rüveyda Çetin demeni istiyorum.");

            Console.Write($"[ASSISTANT]: ");

            await foreach (StreamingChatCompletionUpdate completionUpdate in completionUpdates)
            {
                if (completionUpdate.ContentUpdate.Count > 0)
                {
                    Console.Write(completionUpdate.ContentUpdate[0].Text);
                }
            }
            Console.ReadLine();
        }


    }
}
