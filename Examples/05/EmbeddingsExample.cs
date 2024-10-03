using OpenAI.Embeddings;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyOpenAIProject.Examples._05
{
    public class EmbeddingsExample
    {
        private static readonly string apiKey = ConfigReader.ReadApiKeyFromConfig();

        private static readonly string description = "Bu otobüs firmasıyla yaptığım yolculuk oldukça keyifliydi. Koltuklar rahat ve genişti, ayrıca"
            + " personel çok nazikti. İkramlar yeterliydi ve zamanında varış sağlandı. Güvenli bir yolculuk için kesinlikle"
            + " tavsiye ederim.";

        private static readonly string category = "Lüks";

        public static void SimpleEmbedding()
        {
            EmbeddingClient client = new("text-embedding-3-small", apiKey);
            OpenAIEmbedding embedding = client.GenerateEmbedding(description);
            ReadOnlyMemory<float> vector = embedding.ToFloats();

            Console.WriteLine($"Boyut(Dimension): {vector.Length}");
            Console.WriteLine($"Kayan Noktalı Sayılar(Float): ");
            for (int i = 0; i < vector.Length; i++)
            {
                Console.WriteLine($"  [{i,4}] = {vector.Span[i]}");
            }
        }

        public static async Task SimpleEmbeddingAsync()
        {
            EmbeddingClient client = new("text-embedding-3-small", apiKey);

            OpenAIEmbedding embedding = await client.GenerateEmbeddingAsync(description);
            ReadOnlyMemory<float> vector = embedding.ToFloats();

            Console.WriteLine($"Boyut(Dimension): {vector.Length}");
            Console.WriteLine($"Kayan Noktalı Sayılar(Float): ");
            for (int i = 0; i < vector.Length; i++)
            {
                Console.WriteLine($"  [{i,4}] = {vector.Span[i]}");
            }
        }

        public static void EmbeddingWithOptions()
        {
            EmbeddingClient client = new("text-embedding-3-small", apiKey);

            EmbeddingGenerationOptions options = new() { Dimensions = 512 };

            OpenAIEmbedding embedding = client.GenerateEmbedding(description, options);
            ReadOnlyMemory<float> vector = embedding.ToFloats();

            Console.WriteLine($"Boyut: {vector.Length}");
            Console.WriteLine($"Kayan Noktalı Sayılar: ");
            for (int i = 0; i < vector.Length; i++)
            {
                Console.WriteLine($"  [{i,3}] = {vector.Span[i]}");
            }
        }

        public static async Task EmbeddingWithOptionsAsync()
        {
            EmbeddingClient client = new("text-embedding-3-small", apiKey);
            EmbeddingGenerationOptions options = new() { Dimensions = 512 };

            OpenAIEmbedding embedding = await client.GenerateEmbeddingAsync(description, options);
            ReadOnlyMemory<float> vector = embedding.ToFloats();

            Console.WriteLine($"Boyut: {vector.Length}");
            Console.WriteLine($"Kayan Noktalı Sayılar: ");
            for (int i = 0; i < vector.Length; i++)
            {
                Console.WriteLine($"  [{i,3}] = {vector.Span[i]}");
            }
        }

        public static void MultipleEmbeddings()
        {
            EmbeddingClient client = new("text-embedding-3-small", apiKey);

            List<string> girdiler = new() { category, description };  // Birden fazla girdi ile liste oluşturma

            // Her iki metin için embedding oluşturur
            OpenAIEmbeddingCollection collection = client.GenerateEmbeddings(girdiler);

            // Her embedding'i ayrı ayrı işler ve sonuçları konsola yazdırır
            foreach (OpenAIEmbedding embedding in collection)
            {
                ReadOnlyMemory<float> vector = embedding.ToFloats();

                Console.WriteLine($"Boyut: {vector.Length}");
                Console.WriteLine("Kayan Noktalı Sayılar: ");
                for (int i = 0; i < vector.Length; i++)
                {
                    Console.WriteLine($"  [{i,4}] = {vector.Span[i]}");
                }

                Console.WriteLine(); // Her embedding'in sonucunu ayırmak için boş satır ekleniyor
            }
        }

        public static async Task MultipleEmbeddingsAsync()
        {
            EmbeddingClient client = new("text-embedding-3-small", apiKey);

            List<string> inputs = new() { category, description };  // Girdi listesini oluşturuyoruz

            // Asenkron olarak embedding'leri topluca oluşturur
            OpenAIEmbeddingCollection collection = await client.GenerateEmbeddingsAsync(inputs);

            // Her embedding'i ayrı ayrı işler ve sonuçları konsola yazdırır
            foreach (OpenAIEmbedding embedding in collection)
            {
                ReadOnlyMemory<float> vector = embedding.ToFloats();

                Console.WriteLine($"Boyut: {vector.Length}");
                Console.WriteLine("Kayan Noktalı Sayılar: ");
                for (int i = 0; i < vector.Length; i++)
                {
                    Console.WriteLine($"  [{i,4}] = {vector.Span[i]}");
                }

                Console.WriteLine(); // Her embedding'in sonucunu ayırmak için boş bir satır
            }
        }
    }
}
