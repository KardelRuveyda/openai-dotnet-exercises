using OpenAI;
using OpenAI.Assistants;
using OpenAI.Files;
using System.ClientModel;

namespace MyOpenAIProject.Examples
{
    public class RAGExample
    {
         #pragma warning disable OPENAI001
        static OpenAIClient CreateOpenAIClient()
        {
            // API anahtarını ConfigReader sınıfından alma işlemi
            string apiKey = ConfigReader.ReadApiKeyFromConfig();
            return new OpenAIClient(apiKey);
        }
        static AssistantClient GetAssistantClient(OpenAIClient openAIClient)
        {
            return openAIClient.GetAssistantClient();
        }

        static FileClient GetFileClient(OpenAIClient openAIClient)
        {
            return openAIClient.GetFileClient();
        }

        static Stream LoadDocument()
        {
            return BinaryData.FromString("""
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
        }

        static OpenAIFileInfo UploadDocument(FileClient fileClient, Stream document)
        {
            // Belgeyi OpenAI'ya yükleme
            return fileClient.UploadFile(document, "euro_2024_matches.json", FileUploadPurpose.Assistants);
        }

        static Assistant CreateAssistant(AssistantClient assistantClient, OpenAIFileInfo file)
        {
            AssistantCreationOptions assistantOptions = new()
            {
                Name = "Örnek: Euro 2024 Maç Sonuçları RAG",
                Instructions =
                    "Kullanıcı sorgularına dayalı olarak Euro 2024 maç sonuçlarını arayan bir yardımcısınız. Maç sonucu istendiğinde sana verilen dokümana göre cevap ver.",
                Tools = { new FileSearchToolDefinition(), new CodeInterpreterToolDefinition() },
                ToolResources = new() { FileSearch = new() { NewVectorStores = { new VectorStoreCreationHelper(new[] { file.Id }) } } }
            };
            return assistantClient.CreateAssistant("gpt-4o", assistantOptions);
        }

        static ThreadRun CreateAndRunThread(AssistantClient assistantClient, Assistant assistant)
        {
            ThreadCreationOptions threadOptions = new()
            {
                InitialMessages = { "Türkiye'nin Euro 2024'teki ilk maçında kimle oynadı ve sonuç ne oldu?" }
            };

            return assistantClient.CreateThreadAndRun(assistant.Id, threadOptions);
        }



        static ThreadRun PollThreadRunStatus(AssistantClient assistantClient, ThreadRun threadRun)
        {
            do
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                threadRun = assistantClient.GetRun(threadRun.ThreadId, threadRun.Id);
            } while (!threadRun.Status.IsTerminal);

            return threadRun;
        }
        static void PrintThreadMessages(AssistantClient assistantClient, FileClient fileClient, ThreadRun threadRun)
        {
            PageableCollection<ThreadMessage> messages = assistantClient.GetMessages(threadRun.ThreadId, ListOrder.OldestFirst);

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
                Console.WriteLine();
            }
        }

        public static void Main()
        {
            OpenAIClient openAIClient = CreateOpenAIClient();
            AssistantClient assistantClient = GetAssistantClient(openAIClient);
            FileClient fileClient = GetFileClient(openAIClient);

            Stream document = LoadDocument();

            // JSON belgesini vektör mağazasına yükleme
            OpenAIFileInfo salesFile = UploadDocument(fileClient, document);

            Assistant assistant = CreateAssistant(assistantClient, salesFile);
            ThreadRun threadRun = CreateAndRunThread(assistantClient, assistant);
            threadRun = PollThreadRunStatus(assistantClient, threadRun);
            PrintThreadMessages(assistantClient, fileClient, threadRun);
        }

    }
}
