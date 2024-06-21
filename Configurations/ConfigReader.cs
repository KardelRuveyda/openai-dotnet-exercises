using Newtonsoft.Json.Linq;
using System;
using System.IO;

public class ConfigReader
{
    public static string ReadApiKeyFromConfig()
    {
        try
        {
            string configPath = Path.Combine(Environment.CurrentDirectory, "config.json");
            JObject config = JObject.Parse(File.ReadAllText(configPath));
            return config["OpenAI"]["ApiKey"].ToString();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading API key from config file: {ex.Message}");
            return null;
        }
    }
}
