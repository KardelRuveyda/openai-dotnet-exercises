
using OpenAI.Chat;
using System.Text.Json;

public class StructuredOutputs
{
    // Run metodu, OpenAI API'sini kullanarak yapılandırılmış bir çıktı elde eder ve sonuçları ekrana yazar
    public static async Task Run()
    {
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

        ChatClient client = new(model: "gpt-4o-mini", apiKey);

        // Kullanıcı mesajını oluşturuyoruz
        List<ChatMessage> messages = new()
    {
        new UserChatMessage("How can I solve 8x - 7 = 17?")
    };

        // Yapılandırılmış yanıt formatı oluşturuyoruz (JSON şeması)
        ChatCompletionOptions options = new()
        {
            ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                jsonSchemaFormatName: "math_reasoning",
                jsonSchema: BinaryData.FromBytes("""
                    {
                        "type": "object",
                        "properties": {
                        "steps": {
                            "type": "array",
                            "items": {
                            "type": "object",
                            "properties": {
                                "explanation": { "type": "string" },
                                "output": { "type": "string" }
                            },
                            "required": ["explanation", "output"],
                            "additionalProperties": false
                            }
                        },
                        "final_answer": { "type": "string" }
                        },
                        "required": ["steps", "final_answer"],
                        "additionalProperties": false
                    }
                    """u8.ToArray()),
                jsonSchemaIsStrict: true),

        };

        // OpenAI API'den tamamlama isteğini yapıyoruz
        ChatCompletion completion = await client.CompleteChatAsync(messages, options);

        // Yapılandırılmış JSON'u ayrıştırıyoruz
        using JsonDocument structuredJson = JsonDocument.Parse(completion.Content[0].Text);

        // Nihai cevabı ve adımları konsola yazdırıyoruz
        Console.WriteLine($"Final answer: {structuredJson.RootElement.GetProperty("final_answer").GetString()}");
        Console.WriteLine("Reasoning steps:");

        foreach (JsonElement stepElement in structuredJson.RootElement.GetProperty("steps").EnumerateArray())
        {
            Console.WriteLine($"  - Explanation: {stepElement.GetProperty("explanation").GetString()}");
            Console.WriteLine($"    Output: {stepElement.GetProperty("output").GetString()}");
        }
    }
}
