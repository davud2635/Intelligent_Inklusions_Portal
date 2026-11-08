using Application.Interfaces;
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Embeddings;
using System.Linq;

namespace Infrastructure.Services
{
    public class OllamaService : IAIChatService
    {
        private readonly IChatCompletionService _chatCompletionService;
        private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingService;

        public OllamaService()
        {
            var builder = Kernel.CreateBuilder();
            
            // Tilføj Chat-modellen (llama3.2)
            builder.AddOllamaChatCompletion(
                modelId: "llama3.2",
                endpoint: new Uri("http://localhost:11434")
            );

            // Tilføj Vektor-modellen (all-minilm)
            builder.AddOllamaEmbeddingGenerator(
                modelId: "all-minilm",
                endpoint: new Uri("http://localhost:11434")
            );

            var kernel = builder.Build();
            _chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
            _embeddingService = kernel.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();
        }

        public async Task<string> GetResponseAsync(string userMessage)
        {
            var history = new ChatHistory();
            history.AddSystemMessage("Du er en empatisk assistent for en kommune. Du får udleveret nogle sociale tilbud (kontekst). Brug KUN disse tilbud til at hjælpe borgeren. Vær kort, venlig og direkte.");
            history.AddUserMessage(userMessage);

            var result = await _chatCompletionService.GetChatMessageContentAsync(history);
            return result.Content ?? "Kunne ikke generere svar.";
        }

        public async Task<float[]> GenerateEmbeddingAsync(string text)
        {

            // Kald GenerateAsync, som returnerer et GeneratedEmbeddings objekt
            var result = await _embeddingService.GenerateAsync(new[] {text});

            // Træk det faktiske float-array ud af det første resultat
            var embedding = result.FirstOrDefault()?.Vector.ToArray();
            return embedding ?? Array.Empty<float>();
        }
    }
}