namespace ActIntelligenceService.Domain.Models.InsightProviders
{
    public class TextAnalysisResponse
    {
        public List<Transcript>? Transcripts { get; set; }
        public string? Summary { get; set; }
        public string? KeyPoints { get; set; }
        public string? ActionItems { get; set; }
        public string? Sentiment { get; set; }
    }
}
