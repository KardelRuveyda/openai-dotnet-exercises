
using OpenAI.Chat;
using System.Text.Json;

public class StructuredOutputs
{
    // Run metodu, OpenAI API'sini kullanarak yapılandırılmış bir çıktı elde eder ve sonuçları ekrana yazar
    public static async Task Run()
    {

        // API anahtarını okuyoruz
        string apiKey = ConfigReader.ReadApiKeyFromConfig();
        if (string.IsNullOrEmpty(apiKey))
        {
            Console.WriteLine("API anahtarı bulunamadı.");
            return;
        }


        // OpenAI ChatClient oluşturuluyor
        Console.WriteLine("ChatClient oluşturuluyor...");
        ChatClient client = new(model: "gpt-4", apiKey);

        // Kullanıcıdan bir matematik problemi alıyoruz
        Console.WriteLine("Bir matematik problemi girin: ");
        string mathProblem = Console.ReadLine();

        // Sistem mesajı: Modeli JSON formatında yanıt vermeye zorlamak için
        var systemMessage = new SystemChatMessage("Lütfen şu formatta bir yanıt üret: {\"steps\": [{\"explanation\": \"\", \"output\": \"\"}], \"final_answer\": \"\"}");

        // Kullanıcı mesajı
        var userMessage = new UserChatMessage($"Nasıl çözülür? {mathProblem}");

        // Sohbet tamamlanıyor (JSON yanıtı bekliyoruz)
        ChatCompletionOptions options = new ChatCompletionOptions();
        ChatCompletion chatCompletion = await client.CompleteChatAsync(
            new List<ChatMessage> { systemMessage, userMessage }, options);

        // JSON çıktısını ayrıştırıyoruz
        using JsonDocument structuredJson = JsonDocument.Parse(chatCompletion.ToString());

        // Nihai cevabı yazdırıyoruz
        Console.WriteLine($"Final answer: {structuredJson.RootElement.GetProperty("final_answer").GetString()}");
        Console.WriteLine("Çözüm adımları:");

        // Adımları sırasıyla yazdırıyoruz
        foreach (JsonElement stepElement in structuredJson.RootElement.GetProperty("steps").EnumerateArray())
        {
            Console.WriteLine($"  - Açıklama: {stepElement.GetProperty("explanation").GetString()}");
            Console.WriteLine($"    Sonuç: {stepElement.GetProperty("output").GetString()}");
        }
    }
}
