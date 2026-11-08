using Application.Interfaces;
using Domain.Entities;

namespace Application.Services
{
    public class InklusionService
    {
        private readonly IOfferSearchService _offerSearch;
        private readonly IAIChatService _ai;

        public InklusionService(IOfferSearchService offerSearch, IAIChatService ai)
        {
            _offerSearch = offerSearch;
            _ai = ai;
        }

        public async Task SeedDataAsync()
        {
            if (await _offerSearch.HasAnyOffersAsync()) return;

            var offers = new List<VolunteerOffer>
            {
                new VolunteerOffer { Title = "Skolens Venner", Description = "Frivillige voksne der støtter børn med skolevægring og mistrivsel.", ContactInfo = "skoleven@kommune.dk" },
                new VolunteerOffer { Title = "Gadeplansteamet", Description = "Hjælp til teenagere der hænger ud på gaden og er på vej i kriminalitet.", ContactInfo = "gadeplan@kommune.dk" },
                new VolunteerOffer { Title = "Pårørendegruppen", Description = "Samtalegruppe for forældre til børn med handicap eller psykiske lidelser.", ContactInfo = "paaroerende@kommune.dk" }
            };

            // Orkestrer logikken: Få AI til at lave vektorerne
            foreach (var offer in offers)
            {
                offer.Embedding = await _ai.GenerateEmbeddingAsync($"{offer.Title}: {offer.Description}");
            }

            // Bed infrastrukturen om at gemme dem
            await _offerSearch.SaveOffersAsync(offers);
        }

        public async Task<string> HandleCitizenQueryAsync(string userMessage)
        {
            // 1. Gør borgerens problem til en vektor
            var userVector = await _ai.GenerateEmbeddingAsync(userMessage);

            // 2. Bed infrastrukturen om at finde de semantisk tætteste (uden at kende til SQL!)
            var bestMatches = await _offerSearch.FindClosestOffersAsync(userVector, 2);

            // 3. Byg konteksten
            var contextString = "Her er de relevante tilbud i kommunen:\n";
            foreach (var match in bestMatches)
            {
                contextString += $"- {match.Title}: {match.Description} (Kontakt: {match.ContactInfo})\n";
            }

            // 4. Lad LLaMA 3.2 læse konteksten og svare borgeren
            var finalPrompt = $"Kontekst:\n{contextString}\n\nBorgerens problem: {userMessage}";
            return await _ai.GetResponseAsync(finalPrompt);
        }
    }
}