using OpenAI.Images;

public class ImageGenerateExample
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

        ImageClient client = new(model: "dall-e-3", apiKey);

        string prompt = @"
Rozet Tasarımı

Kurallar:
🔹 Boyut: 512x512
🔹 Stil: Realistik — bu stilin tüm karakteristik özellikleri (renk geçişleri, ışık-gölge oyunları, detay seviyesi) korunmalıdır.
🔹 Tema: Oyunlaştırılmış — ikonun ifadesine, duygusuna ve sembollerine doğrudan yansıtılmalıdır.
🔹 Arka Plan: Saydam. Arka plan saydam olmalıdır ve tasarımı destekleyecek herhangi bir doku içermemelidir.
🔹 Genel Renk Paleti:
- Birincil: #0062FF
- İkincil: #CDE9FF
- Üçüncül: #FFA933
🔹 Çerçeve: Arma / Kalkan. Tasarım sabit bir arma / kalkan içerisinde konumlandırılmalı ve bunun dışına taşmamalıdır.

🎯 Önemli Kurallar:
- Rozet adı, görsel tasarımda açıkça görünmelidir: Technical PO.
- Ad okunabilir, net ve dengeli bir biçimde tasarımın içinde yer almalıdır.
- Rozet ikonları aynı görsel çizgide ve bütünlükte olmalıdır.
- Tasarımlar sade, modern ve görsel olarak dengeli olmalıdır.
- Yüksek çözünürlükte ve hızlı üretime uygun yapıda olmalıdır.
- Gölgelendirme, yumuşak kenarlar ve minimal detaylar kullanılabilir ancak sadelik korunmalıdır.
- Renk dışına çıkılmamalı, görsel karmaşadan kaçınılmalıdır.
- İkon, kendi başına da bir mesaj iletebilmeli ve anlamlı bir bütünlük taşımalıdır.

Rozet Adı: Technical PO
Gereksinimler: Minimum 2 yıl boyunca en az bir uygulama veya modülde PO olarak görev almış / alıyor olmak.
";


        ImageGenerationOptions options = new()
        {
            Quality = GeneratedImageQuality.High,
            Size = GeneratedImageSize.W1024xH1024, // Dall-E için geçerli boyutlardan biri
            Style = GeneratedImageStyle.Vivid,
            ResponseFormat = GeneratedImageFormat.Bytes
        };

        Console.WriteLine("Rozet oluşturuluyor...");
        GeneratedImage image = client.GenerateImage(prompt, options);
        BinaryData bytes = image.ImageBytes;

        string fileName = $"badge_{Guid.NewGuid()}.png";
        using FileStream stream = File.OpenWrite(fileName);
        bytes.ToStream().CopyTo(stream);

        Console.WriteLine($"Rozet oluşturuldu: {fileName}");
    }
}
