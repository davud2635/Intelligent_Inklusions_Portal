using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Application.Interfaces;

namespace Infrastructure
{
    public class OllamaService : IAIChatService
    {
        private readonly Kernel _kernel;

        public OllamaService()
        {
            var builder = Kernel.CreateBuilder();
            //1. Tilføj din LLaMA-model til chat completion via Ollama
            builder.AddOllamaChatCompletion(modelId: "llama3.2", endpoint: new Uri("http://localhost:11434"));
            builder.AddOllamaEmbeddingGenerator(modelId: "all-minilm",endpoint: new Uri("http://localhost:11434"));
            _kernel = builder.Build();



        }

        public async Task<string> AskAsync(string prompt) 
        {
            var chatService = _kernel.GetRequiredService<IChatCompletionService>();
            var response = await chatService.GetChatMessageContentsAsync(prompt);
            var firstMessage = response.First();
            var content = firstMessage.Content;

            return content ?? string.Empty;


        }





    }
      
}
