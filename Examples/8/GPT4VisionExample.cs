using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.IO;


public class GPT4VisionExample
{
    public static void RunAssistantWithVision()
    {
        // 1. API anahtarını config.json dosyasından oku
        string apiKey = ConfigReader.ReadApiKeyFromConfig();

        // 2. API anahtarı alınamazsa işlemi sonlandır
        if (string.IsNullOrEmpty(apiKey))
        {
            Console.WriteLine("config.json dosyasında API anahtarı bulunamadı.");
            return;
        }

        // 3. GPT-4o modelini kullanacak bir ChatClient nesnesi oluştur
        ChatClient client = new("gpt-4o", apiKey);

        // 4. Görüntü dosyasının yolunu belirt
        string imageFilePath = Path.Combine("Assets", "images_dog_and_cat.png");

        // 5. Görüntü dosyasını okuyarak bir stream aç ve byte verisi olarak al
        using Stream imageStream = File.OpenRead(imageFilePath);
        BinaryData imageBytes = BinaryData.FromStream(imageStream);

        // 6. Kullanıcıdan gelen bir mesaj listesi oluştur ve resim verisini ekle
        List<ChatMessage> messages = new List<ChatMessage>
            {
                new UserChatMessage(
                    ChatMessageContentPart.CreateTextPart("Lütfen aşağıdaki görüntüyü tanımlar mısın?"),
                    ChatMessageContentPart.CreateImagePart(imageBytes, "image/png")
                ),
            };

        // 7. OpenAI'ye görüntü ve mesajı gönderip tamamlanmış bir yanıt al
        ChatCompletion completion = client.CompleteChat(messages);

        // 8. Asistanın yanıtını ekrana yazdır
        Console.WriteLine($"[ASİSTAN]: {completion.Content[0].Text}");
    }
}
