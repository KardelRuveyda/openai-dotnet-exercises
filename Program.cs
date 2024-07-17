// See https://aka.ms/new-console-template for more information
using MyOpenAIProject.Examples;
using OpenAI.Examples;
//ChatExample.RunAsync().Wait();
//ChatExample.Run();
//AudioExample.Example01_SimpleTextToSpeech();
//StreamingChatExample.Run();
//ToolsAndFunctionsExample.Run();
//ImageGenerateExample.Run();
//RAGExample.Main();
AssistantWithVisionExample.Main("merhaba ilgili resimi link ile beraber gönderiyorum.",
                                new Uri("https://fastly.picsum.photos/id/1/5000/3333.jpg?hmac=Asv2DU3rA_5D1xSe22xZK47WEAN0wjWeFOhzd13ujW4"));