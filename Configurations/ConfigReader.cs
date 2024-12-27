using Newtonsoft.Json.Linq;
using System;
using System.IO;

public class ConfigReader
{
    public static string ReadApiKeyFromConfig()
    {
        try
        {
            // Klasör yoluna göre config dosyasýnýn tam yolunu ayarlayýn
            string projectRoot = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName;

            string configPath = Path.Combine(projectRoot, "openai-dotnet-exercises", "config.json");


            // config.json dosyasýnýn var olup olmadýðýný kontrol et
            if (!File.Exists(configPath))
            {
                Console.WriteLine("Config file not found.");
                return null;
            }

            // Dosyayý oku ve JSON olarak parse et
            JObject config = JObject.Parse(File.ReadAllText(configPath));

            // "OpenAI" anahtarýnýn ve "ApiKey" anahtarýnýn olup olmadýðýný kontrol et
            if (config["OpenAI"]?["ApiKey"] == null)
            {
                Console.WriteLine("API key not found in config file.");
                return null;
            }

            // API anahtarýný döndür
            return config["OpenAI"]["ApiKey"].ToString();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading API key from config file: {ex.Message}");
            return null;
        }
    }
}
