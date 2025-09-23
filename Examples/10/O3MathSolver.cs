using OpenAI.Chat;

namespace OpenAI.Examples
{
    public class O3MathSolver
    {
        public static void Example_O1_MathProblemSolver()
        {
            // API anahtarını config'den oku
            string apiKey = ConfigReader.ReadApiKeyFromConfig();

            if (string.IsNullOrEmpty(apiKey))
            {
                Console.WriteLine("API anahtarı bulunamadı.");
                return;
            }

            // O1-preview modeli ile ChatClient oluştur
            ChatClient client = new("o3-mini", apiKey);

            // Sistem talimatları için SystemChatMessage kullan
            List<ChatMessage> messages = new()
            {
                ChatMessage.CreateSystemMessage(@"
                    Sen bir matematik uzmanısın. Problemleri şu şekilde çöz:
                    1. Problemi analiz et
                    2. Hangi yöntemi kullanacağını belirle  
                    3. Adım adım çözüm yap
                    4. Sonucu doğrula
                    5. Alternatif çözüm yolları varsa belirt
                "),
                ChatMessage.CreateUserMessage(@"
                    Aşağıdaki matematik problemini çöz:
                    
                    Bir şirket, aylık satışlarının %15'ini reklam harcamalarına ayırıyor.
                    Bu ay toplam 120.000 TL satış yaptılar ve reklam harcamaları
                    geçen aya göre %20 arttı. Geçen ay reklam harcamaları
                    ne kadardı?
                ")
            };

            // O1 modeli için özel seçenekler
            ChatCompletionOptions options = new()
            {
                MaxOutputTokenCount = 2000
            };

            try
            {
                Console.WriteLine("Problem çözülüyor... (O1 modeli düşünüyor)");

                ChatCompletion completion = client.CompleteChat(messages, options);

                // Reasoning token kullanımını göster
                if (completion.Usage?.OutputTokenDetails != null)
                {
                    Console.WriteLine("\n=== TOKEN KULLANIMI ===");
                    Console.WriteLine($"🧠 Reasoning Tokens: {completion.Usage.OutputTokenDetails.ReasoningTokenCount}");
                    Console.WriteLine($"💬 Display Tokens: {completion.Usage.OutputTokenCount - completion.Usage.OutputTokenDetails.ReasoningTokenCount}");
                    Console.WriteLine($"📊 Total Output Tokens: {completion.Usage.OutputTokenCount}");
                    Console.WriteLine($"📥 Input Tokens: {completion.Usage.InputTokenCount}");
                }

                Console.WriteLine("\n=== ÇÖZÜM ===");
                Console.WriteLine(completion.Content[0].Text);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata oluştu: {ex.Message}");
            }
        }
    }
}