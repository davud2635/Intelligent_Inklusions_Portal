namespace Domain.Entities
{
    public class VolunteerOffer
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string ContactInfo { get; set; } = string.Empty;

        // Denne property skal rumme vektor-repræsentationen af beskrivelsen (fra Ollama).
        // I Entity Framework Core 9 til SQL Server bruges typisk et float array eller 
        // ReadOnlyMemory<float> til at gemme vektorer (embeddings) til AI-søgning.
        public float[]? Embedding { get; set; }
    }
}