using OpenAI.Chat;
using System.ClientModel;

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

            ChatClient client = new(model: "gpt-4o", apiKey);

            CollectionResult<StreamingChatCompletionUpdate> completionUpdates =
                client.CompleteChatStreaming("'Bu bir testtir.' demeni istiyorum.");

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
                client.CompleteChatStreamingAsync("'Bu bir testtir.' demeni istiyorum.");

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
