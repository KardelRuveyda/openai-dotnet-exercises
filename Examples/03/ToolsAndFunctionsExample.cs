using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyOpenAIProject.Examples
{
    public class ToolsAndFunctionsExample
    {
        private static string GetCurrentLocation()
        {
            Console.WriteLine("---- Anlık lokasyon çağırıldı.----");
            // Normalde burada konum API'si çağrılır, biz İstanbul'u sabit olarak döndüreceğiz.
            return "İstanbul";
        }

        private static string GetCurrentWeather(string location, string unit = "celsius")
        {
            Console.WriteLine("---- Hava durumu çağırıldı.----");   
            // Hava durumu API'si çağrısı yapılır. Örneğin, "Bugün İstanbul'da hava 25 derece."
            return $"25 {unit} ve İstanbul'da açık bir hava var.";
        }

        private static readonly ChatTool getCurrentLocationTool = ChatTool.CreateFunctionTool(
            functionName: nameof(GetCurrentLocation),
            functionDescription: "Kullanıcının mevcut konumunu alır ve bu örnekte İstanbul olarak sabitler."
        );

        private static readonly ChatTool getCurrentWeatherTool = ChatTool.CreateFunctionTool(
            functionName: nameof(GetCurrentWeather),
            functionDescription: "Belirli bir konum için hava durumu bilgisini alır. İstanbul varsayılan konumdur.",
            functionParameters: BinaryData.FromString("""
        {
            "type": "object",
            "properties": {
                "location": {
                    "type": "string",
                    "description": "Konum bilgisi, örn. İstanbul"
                },
                "unit": {
                    "type": "string",
                    "enum": [ "celsius", "fahrenheit" ],
                    "description": "Kullanılacak sıcaklık birimi, varsayılan 'celsius'."
                }
            },
            "required": [ "location" ]
        }
        """)
        );

        public static void Run()
        {
            Console.WriteLine("Sohbet başlatılıyor...");

            // Kullanıcıdan girdi alınması
            Console.WriteLine("Kullanıcı: İstanbul'da hava nasıl diye sorabilir.");
            string userInput = Console.ReadLine();

            // Kullanıcı mesajını ekliyoruz
            List<ChatMessage> messages = new()
            {
                new UserChatMessage(userInput)  // Kullanıcıdan gelen mesaj
            };

            // Hangi araçların kullanılacağını belirleyen seçenekler
            ChatCompletionOptions options = new()
            {
                Tools = { getCurrentLocationTool, getCurrentWeatherTool }  // Araçları tanımlıyoruz
            };

            // API anahtarını config dosyasından okuma işlemi
            Console.WriteLine("API anahtarı okunuyor...");
            string apiKey = ConfigReader.ReadApiKeyFromConfig();

            if (string.IsNullOrEmpty(apiKey))
            {
                Console.WriteLine("Hata: API anahtarı bulunamadı.");
                return;
            }

            // OpenAI ChatClient oluşturuluyor
            Console.WriteLine("ChatClient oluşturuluyor...");
            ChatClient client = new(model: "gpt-4o", apiKey);


            bool requiresAction;

            do
            {
                requiresAction = false;
                ChatCompletion chatCompletion = client.CompleteChat(messages, options);

                switch (chatCompletion.FinishReason)
                {
                    case ChatFinishReason.Stop:
                        {
                            // Add the assistant message to the conversation history.
                            // Asistan cevabını ekle
                            messages.Add(new AssistantChatMessage(chatCompletion));

                            // chatCompletion nesnesini kontrol etme
                            string jsonResponse = JsonSerializer.Serialize(chatCompletion);
                            break;
                        }

                    case ChatFinishReason.ToolCalls:
                        {
                            messages.Add(new AssistantChatMessage(chatCompletion));

                            foreach (ChatToolCall toolCall in chatCompletion.ToolCalls)
                            {
                                switch (toolCall.FunctionName)
                                {
                                    case nameof(GetCurrentLocation):
                                        {
                                            string toolResult = GetCurrentLocation();
                                            messages.Add(new ToolChatMessage(toolCall.Id, toolResult));
                                            break;
                                        }

                                    case nameof(GetCurrentWeather):
                                        {

                                            using JsonDocument argumentsJson = JsonDocument.Parse(toolCall.FunctionArguments);
                                            bool hasLocation = argumentsJson.RootElement.TryGetProperty("location", out JsonElement location);
                                            bool hasUnit = argumentsJson.RootElement.TryGetProperty("unit", out JsonElement unit);

                                            if (!hasLocation)
                                            {
                                                throw new ArgumentNullException(nameof(location), "The location argument is required.");
                                            }

                                            string toolResult = hasUnit
                                                ? GetCurrentWeather(location.GetString(), unit.GetString())
                                                : GetCurrentWeather(location.GetString());
                                            messages.Add(new ToolChatMessage(toolCall.Id, toolResult));
                                            break;
                                        }

                                    default:
                                        {
                                            // Handle other unexpected calls.
                                            throw new NotImplementedException();
                                        }
                                }
                            }

                            requiresAction = true;
                            break;
                        }

                    case ChatFinishReason.Length:
                        throw new NotImplementedException("MaxTokens parametresi veya token limitinin aşılması nedeniyle tamamlanmamış model çıktısı.");

                    case ChatFinishReason.ContentFilter:
                        throw new NotImplementedException("İçerik filtresi bayrağı nedeniyle atlanan içerik.");

                    case ChatFinishReason.FunctionCall:
                        throw new NotImplementedException("Araç çağrıları lehine kullanım dışı bırakıld.");

                    default:
                        throw new NotImplementedException(chatCompletion.FinishReason.ToString());
                }
            } while (requiresAction);

            // Mesajları konsola yazdırıyoruz
            Console.WriteLine("Sohbet tamamlandı. Sonuçlar konsola yazdırılıyor...");
            foreach (ChatMessage message in messages)
            {
                if (message is UserChatMessage userMessage)
                {
                    Console.WriteLine($"[KULLANICI]: {userMessage.Content[0].Text}");
                }
                else if (message is AssistantChatMessage assistantMessage)
                {
                    // Asistan yanıtı Content[0].Text alanında yer alıyor, bu nedenle doğru şekilde yazdırıyoruz
                    if (assistantMessage.Content != null && assistantMessage.Content.Count > 0)
                    {
                        Console.WriteLine($"[ASİSTAN]: {assistantMessage.Content[0].Text}");
                    }
                }
                else if (message is ToolChatMessage toolMessage)
                {
                    Console.WriteLine($"[ARAÇ]: {toolMessage.Content[0].Text}");
                }
            }

        }

    }
}
