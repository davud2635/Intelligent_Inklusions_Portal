using Domain.Entities;

namespace Application.Interfaces
{
    public interface IOfferSearchService
    {
        Task<bool> HasAnyOffersAsync();
        Task SaveOffersAsync(IEnumerable<VolunteerOffer> offers);
        Task<List<VolunteerOffer>> FindClosestOffersAsync(float[] vector, int limit = 2);
    }
}