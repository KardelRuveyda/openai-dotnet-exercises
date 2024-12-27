using Newtonsoft.Json.Linq;
using System;
using System.IO;

public class ConfigReader
{
    public static string ReadApiKeyFromConfig()
    {
        try
        {
            // Klas�r yoluna g�re config dosyas�n�n tam yolunu ayarlay�n
            string projectRoot = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName;

            string configPath = Path.Combine(projectRoot, "openai-dotnet-exercises", "config.json");


            // config.json dosyas�n�n var olup olmad���n� kontrol et
            if (!File.Exists(configPath))
            {
                Console.WriteLine("Config file not found.");
                return null;
            }

            // Dosyay� oku ve JSON olarak parse et
            JObject config = JObject.Parse(File.ReadAllText(configPath));

            // "OpenAI" anahtar�n�n ve "ApiKey" anahtar�n�n olup olmad���n� kontrol et
            if (config["OpenAI"]?["ApiKey"] == null)
            {
                Console.WriteLine("API key not found in config file.");
                return null;
            }

            // API anahtar�n� d�nd�r
            return config["OpenAI"]["ApiKey"].ToString();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading API key from config file: {ex.Message}");
            return null;
        }
    }
}
