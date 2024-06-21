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
                // Kullanıcının mevcut konumunu almak için burada konum API'sını çağırın.
                return "İstanbul";
            }

            private static string GetCurrentWeather(string location, string unit = "celsius")
            {
                // Verilen konum için hava durumunu sorgulamak için burada hava durumu API'sını çağırın.
                return $"30 {unit}";
            }

            private static readonly ChatTool getCurrentLocationTool = ChatTool.CreateFunctionTool(
                functionName: nameof(GetCurrentLocation),
                functionDescription: "Kullanıcının mevcut konumunu alır"
            );

            private static readonly ChatTool getCurrentWeatherTool = ChatTool.CreateFunctionTool(
                functionName: nameof(GetCurrentWeather),
                functionDescription: "Verilen konum için mevcut hava durumunu alır",
                functionParameters: BinaryData.FromString(@"
            {
                ""type"": ""object"",
                ""properties"": {
                    ""location"": {
                        ""type"": ""string"",
                        ""description"": ""Şehir ve eyalet, örn. İstanbul, TR""
                    },
                    ""unit"": {
                        ""type"": ""string"",
                        ""enum"": [ ""celsius"", ""fahrenheit"" ],
                        ""description"": ""Kullanılacak sıcaklık birimi. Belirtilen konumdan bu çıkarılır.""
                    }
                },
                ""required"": [ ""location"" ]
            }
        ")
            );

        public static void Run()
        {
            Console.WriteLine("Sohbet başlatılıyor...");

            List<ChatMessage> messages = new()
    {
        new UserChatMessage("Bugün hava nasıl?")
    };

            ChatCompletionOptions options = new()
            {
                Tools = { getCurrentLocationTool, getCurrentWeatherTool }
            };

            // API anahtarını ConfigReader sınıfından alma işlemi
            Console.WriteLine("API anahtarı okunuyor...");
            string apiKey = ConfigReader.ReadApiKeyFromConfig();

            // API anahtarı alınamazsa işlemi sonlandır
            if (string.IsNullOrEmpty(apiKey))
            {
                Console.WriteLine("Hata: API anahtarı bulunamadı.");
                return;
            }

            // OpenAI ChatClient oluşturun
            Console.WriteLine("ChatClient oluşturuluyor...");
            ChatClient client = new(model: "gpt-4", apiKey);

            Console.WriteLine("Chat tamamlama başlatılıyor...");

            ChatCompletion chatCompletion = client.CompleteChat(messages, options);

            switch (chatCompletion.FinishReason)
            {
                case ChatFinishReason.Stop:
                    {
                        Console.WriteLine("Sohbet tamamlandı. Sonuçlar:");
                        messages.Add(new AssistantChatMessage(chatCompletion));
                        break;
                    }

                case ChatFinishReason.ToolCalls:
                    {
                        Console.WriteLine("Araç çağrıları yapılıyor...");
                        messages.Add(new AssistantChatMessage(chatCompletion));

                        foreach (ChatToolCall toolCall in chatCompletion.ToolCalls)
                        {
                            switch (toolCall.FunctionName)
                            {
                                case nameof(GetCurrentLocation):
                                    {
                                        Console.WriteLine("Konum alınıyor...");
                                        string toolResult = GetCurrentLocation();
                                        messages.Add(new ToolChatMessage(toolCall.Id, toolResult));
                                        break;
                                    }

                                case nameof(GetCurrentWeather):
                                    {
                                        Console.WriteLine("Hava durumu sorgulanıyor...");
                                        using JsonDocument argumentsJson = JsonDocument.Parse(toolCall.FunctionArguments);
                                        bool hasLocation = argumentsJson.RootElement.TryGetProperty("location", out JsonElement location);
                                        bool hasUnit = argumentsJson.RootElement.TryGetProperty("unit", out JsonElement unit);

                                        if (!hasLocation)
                                        {
                                            throw new ArgumentNullException(nameof(location), "Konum argümanı gereklidir.");
                                        }

                                        string toolResult = hasUnit
                                            ? GetCurrentWeather(location.GetString(), unit.GetString())
                                            : GetCurrentWeather(location.GetString());
                                        messages.Add(new ToolChatMessage(toolCall.Id, toolResult));
                                        break;
                                    }

                                default:
                                    {
                                        throw new NotImplementedException();
                                    }
                            }
                        }
                        break;
                    }

                case ChatFinishReason.Length:
                    throw new NotImplementedException("MaxTokens parametresi veya token limiti nedeniyle tamamlanmayan model çıktısı.");

                case ChatFinishReason.ContentFilter:
                    throw new NotImplementedException("İçerik filtresi bayrağı nedeniyle atlanan içerik.");

                case ChatFinishReason.FunctionCall:
                    throw new NotImplementedException("Araç çağrılarına göre kullanımdan kaldırıldı.");

                default:
                    throw new NotImplementedException(chatCompletion.FinishReason.ToString());
            }

            Console.WriteLine("Sohbet tamamlandı. Sonuçlar konsola yazdırılıyor...");
            foreach (ChatMessage message in messages)
            {
                if (message is UserChatMessage userMessage)
                {
                    Console.WriteLine($"[USER]: {userMessage.Content[0]}");
                }
                else if (message is ToolChatMessage toolMessage)
                {
                    Console.WriteLine($"[TOOL]: {toolMessage.Content[0]}");
                }
                else
                {
                    Console.WriteLine("[UNKNOWN MESSAGE]");
                }
            }

        }
    }
}
