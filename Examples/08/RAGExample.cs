using OpenAI;
using OpenAI.Assistants;
using OpenAI.Files;
using System.ClientModel;

#pragma warning disable OPENAI001

public class RAGExample
{
    public static void CreateAndRunGenerativeAIAssistant()
    {
        // API key oku
        string apiKey = ConfigReader.ReadApiKeyFromConfig();

        if (string.IsNullOrEmpty(apiKey))
        {
            Console.WriteLine("API key not found in config.json");
            return;
        }

        // OpenAI client'ları başlat
        OpenAIClient openAIClient = new(apiKey);
        OpenAIFileClient fileClient = openAIClient.GetOpenAIFileClient();
        AssistantClient assistantClient = openAIClient.GetAssistantClient();

        // Belgeyi hazırla ve yükle
        using Stream document = BinaryData.FromString("""
        {
            "title": "Generative AI for .NET Developers",
            "sections": [
                {
                    "topic": "RAG (Retrieval-Augmented Generation)",
                    "content": "RAG yaklaşımı, LLM modellerine ek bağlam sağlayarak daha doğru ve güncel yanıtlar üretmesini sağlar. .NET geliştiricileri, belgeleri chunk'lara ayırıp embedding'lerini çıkararak vektör veritabanına yükler. Daha sonra kullanıcı sorgusu embed edilir ve en yakın parçalar modele verilir."
                },
                {
                    "topic": "Prompt Engineering",
                    "content": "İyi bir prompt, sistem mesajı + kullanıcı mesajı + bağlam parçalarını içerir. Net talimatlar ve format beklentileriyle modelden daha kontrollü sonuçlar alınır."
                },
                {
                    "topic": "Kod Örneği",
                    "content": "OpenAI .NET SDK veya HttpClient kullanarak Chat API çağrısı yapılabilir. Örneğin gpt-4o-mini modelinden RAG senaryoları için hızlı ve ekonomik sonuçlar alınabilir."
                }
            ]
        }
        """).ToStream();

        OpenAIFile guideFile = fileClient.UploadFile(
            document,
            "generative_ai_dotnet.json",
            FileUploadPurpose.Assistants);

        // Asistan oluşturma seçenekleri
        AssistantCreationOptions assistantOptions = new()
        {
            Name = "Generative AI .NET Eğitim Asistanı",
            Instructions = "Sen, 'Generative AI for .NET Developers' eğitim belgesiyle ilgili sorulara cevap veren bir yardımcısın. " +
                           "Belge dışındaki sorulara cevap verme!",
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
                        new VectorStoreCreationHelper([guideFile.Id]),
                    }
                }
            },
        };

        Assistant assistant = assistantClient.CreateAssistant("gpt-4o-mini", assistantOptions);

        // Kullanıcıdan soru al - cevap döngüsü
        while (true)
        {
            Console.WriteLine("\nSoru girin (çıkış için 'exit'): ");
            string userQuestion = Console.ReadLine();

            if (userQuestion?.ToLower() == "exit")
                break;

            ThreadCreationOptions threadOptions = new()
            {
                InitialMessages = { userQuestion }
            };

            ThreadRun threadRun = assistantClient.CreateThreadAndRun(assistant.Id, threadOptions);

            // Yanıt tamamlanana kadar bekle
            Console.Write("Asistan yanıt üretiyor");
            do
            {
                Console.Write("."); // her saniye bir nokta ekle
                Thread.Sleep(TimeSpan.FromSeconds(1));
                threadRun = assistantClient.GetRun(threadRun.ThreadId, threadRun.Id);
            } while (!threadRun.Status.IsTerminal);
            Console.WriteLine();

            // Yanıtı yazdır
            CollectionResult<ThreadMessage> messages =
                assistantClient.GetMessages(threadRun.ThreadId, new MessageCollectionOptions() { Order = MessageCollectionOrder.Ascending });

            foreach (ThreadMessage message in messages)
            {
                Console.Write($"[{message.Role.ToString().ToUpper()}]: ");
                foreach (MessageContent contentItem in message.Content)
                {
                    if (!string.IsNullOrEmpty(contentItem.Text))
                    {
                        Console.WriteLine(contentItem.Text);
                    }
                }
            }
        }

        // Kaynak temizleme (isteğe bağlı)
        //_ = assistantClient.DeleteAssistant(assistant.Id);
        //_ = fileClient.DeleteFile(guideFile.Id);
    }
}

#pragma warning restore OPENAI001
