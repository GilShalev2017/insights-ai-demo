using ActIntelligenceService.Domain.Models.InsightProviders;

namespace FRServer.Models
{
    public class InsightsRequest
    {
        public string? VideoFileName { get; set; }
        //public string FacesFolder { get; set; }
        public List<Transcript>? Transcripts { get; set;}
        public string TargetLanguage { get; set; }
    }
}
