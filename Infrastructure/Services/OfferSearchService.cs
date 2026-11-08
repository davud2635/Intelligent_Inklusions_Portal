using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Numerics.Tensors;

namespace Infrastructure.Services
{
    public class OfferSearchService : IOfferSearchService
    {
        private readonly ApplicationDbContext _db;

        public OfferSearchService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<bool> HasAnyOffersAsync()
        {
            return await _db.VolunteerOffers.AnyAsync();
        }

        public async Task SaveOffersAsync(IEnumerable<VolunteerOffer> offers)
        {
            _db.VolunteerOffers.AddRange(offers);
            await _db.SaveChangesAsync();
        }

        public async Task<List<VolunteerOffer>> FindClosestOffersAsync(float[] vector, int limit = 2)
        {
            // 1. Hent alle tilbud ind fra databasen
            var allOffers = await _db.VolunteerOffers
                .Where(o => o.Embedding != null)
                .ToListAsync();

            // 2. Udregn "Cosine Similarity" i C# hukommelsen.
            // CosineSimilarity giver en score op til 1.0 (jo tættere på 1, jo bedre match).
            // Derfor bruger vi OrderByDescending for at få de højeste scores først.
            var bestMatches = allOffers
                .OrderByDescending(o => TensorPrimitives.CosineSimilarity(o.Embedding!, vector))
                .Take(limit)
                .ToList();

            return bestMatches;
        }
    }
}