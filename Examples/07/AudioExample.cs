using OpenAI.Audio;
namespace OpenAI.Examples
{
    public class AudioExample
    {
        public static void SimpleSpeechToText()
        {
            string apiKey = ConfigReader.ReadApiKeyFromConfig();

            AudioClient client = new(model: "whisper-1", apiKey);

            string audioFilePath = Path.Combine("Audio", "Nova.mp3");

            AudioTranscriptionOptions options = new()
            {
                ResponseFormat = AudioTranscriptionFormat.Verbose,
            };

            AudioTranscription transcription = client.TranscribeAudio(audioFilePath, options);

            Console.WriteLine("Transcription:");
            Console.WriteLine($"{transcription.Text}");

            Console.WriteLine();
            Console.WriteLine($"Words:");
            foreach (TranscribedWord word in transcription.Words)
            {
                Console.WriteLine($"  {word.Word,15} : {word.StartTime.TotalMilliseconds,5:0} - {word.EndTime.TotalMilliseconds,5:0}");
            }

            Console.WriteLine();
            Console.WriteLine($"Segments:");
            foreach (TranscribedSegment segment in transcription.Segments)
            {
                Console.WriteLine($"  {segment.Text,90} : {segment.StartTime.TotalMilliseconds,5:0} - {segment.EndTime.TotalMilliseconds,5:0}");
            }
        }



        public static void SimpleTextToSpeech()
        {
            string apiKey = ConfigReader.ReadApiKeyFromConfig();

            AudioClient client = new("tts-1", apiKey);

            string input = "Bu kitap Developer Summit için"
                + " okuyuculara hazırlanmış ,"
                + " Open AI API "
                + " ile ilgili"
                + " çevrimiçi bir kitaptır.";

            BinaryData speech = client.GenerateSpeech(input, GeneratedSpeechVoice.Alloy);

            using FileStream stream = File.OpenWrite($"{Guid.NewGuid()}.mp3");
            speech.ToStream().CopyTo(stream);
        }
    }
}
