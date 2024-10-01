using OpenAI;
using OpenAI.Assistants;
using OpenAI.Files;
using System.ClientModel;

#pragma warning disable OPENAI001

public class RAGExample
{
    public static void CreateAndRunEuro2024Assistant()
    {
        // OpenAI istemcilerini başlat
        string apiKey = ConfigReader.ReadApiKeyFromConfig();

        // API anahtarı alınamazsa işlemi sonlandır
        if (string.IsNullOrEmpty(apiKey))
        {
            Console.WriteLine("API key not found in config.json");
            return;
        }

        OpenAIClient openAIClient = new(apiKey);
        FileClient fileClient = openAIClient.GetFileClient();
        AssistantClient assistantClient = openAIClient.GetAssistantClient();

        // JSON belgesini oluştur ve dosyaya yükle
        using Stream document = BinaryData.FromString("""
        {
            "description": "Bu belge, Euro 2024'e katılan takımların maç sonuçlarını içermektedir.",
            "matches": [
                {
                    "date": "2024-06-10",
                    "team1": "Türkiye",
                    "team2": "Gürcistan",
                    "score": "3-1"
                },
                {
                    "date": "2024-06-12",
                    "team1": "Almanya",
                    "team2": "Fransa",
                    "score": "1-1"
                },
                {
                    "date": "2024-06-15",
                    "team1": "İspanya",
                    "team2": "Portekiz",
                    "score": "3-2"
                }
            ]
        }
        """).ToStream();

        OpenAIFile matchesFile = fileClient.UploadFile(
            document,
            "euro2024_results.json",
            FileUploadPurpose.Assistants);

        // Yardımcı oluşturma seçeneklerini tanımla
        AssistantCreationOptions assistantOptions = new()
        {
            Name = "Euro 2024 Maç Sonuçları Yardımcısı",
            Instructions = "Sen, Euro 2024 maç verileri ile ilgili sorulara cevap veren bir yardımcısın. Grafik ve görsel yerine sadece metin cevapları ver.",
            Tools =
            {
                new FileSearchToolDefinition(),
            },
            ToolResources = new()
            {
                FileSearch = new()
                {
                    NewVectorStores =
                    {
                        new VectorStoreCreationHelper([matchesFile.Id]),
                    }
                }
            },
        };

        // Yardımcıyı oluştur
        Assistant assistant = assistantClient.CreateAssistant("gpt-4o", assistantOptions);

        while (true)
        {
            // Konsoldan soru al
            Console.WriteLine("Sorunuzu girin (Çıkmak için 'exit' yazın):");
            string userQuestion = Console.ReadLine();

            if (userQuestion.ToLower() == "exit")
                break;

            // İleti dizisi oluştur ve çalıştır
            ThreadCreationOptions threadOptions = new()
            {
                InitialMessages = { userQuestion }
            };

            ThreadRun threadRun = assistantClient.CreateThreadAndRun(assistant.Id, threadOptions);

            // Yardımcı tamamlanana kadar bekle
            do
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                threadRun = assistantClient.GetRun(threadRun.ThreadId, threadRun.Id);
            } while (!threadRun.Status.IsTerminal);

            // Mesajları al ve ekrana yazdır
            CollectionResult<ThreadMessage> messages = assistantClient.GetMessages(threadRun.ThreadId, new MessageCollectionOptions() { Order = MessageCollectionOrder.Ascending });

            foreach (ThreadMessage message in messages)
            {
                Console.Write($"[{message.Role.ToString().ToUpper()}]: ");
                foreach (MessageContent contentItem in message.Content)
                {
                    if (!string.IsNullOrEmpty(contentItem.Text))
                    {
                        Console.WriteLine($"{contentItem.Text}");
                    }
                }
            }
        }
    }
}
