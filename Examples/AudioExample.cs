using OpenAI.Audio;
using System;
using System.IO;

namespace OpenAI.Examples
{
    public class AudioExample
    {
        public static void Example01_SimpleTextToSpeech()
        {
            AudioClient client = new AudioClient("tts-1", ConfigReader.ReadApiKeyFromConfig());

            string input = "Euro 2024, Avrupa Futbol Şampiyonası'nın 2024 yılında Almanya'da düzenlenecek olan 17. turnuvasıdır. "
                + "Turnuva, 20 Haziran ile 14 Temmuz 2024 tarihleri arasında gerçekleşecektir. Bu turnuva, Almanya'nın ikinci kez ev sahipliği yapacağı "
                + "Avrupa Futbol Şampiyonası olacaktır.";

            BinaryData speech = client.GenerateSpeechFromText(input, GeneratedSpeechVoice.Alloy);

            using FileStream stream = File.OpenWrite($"{Guid.NewGuid()}.mp3");
            speech.ToStream().CopyTo(stream);
        }
    }
}
