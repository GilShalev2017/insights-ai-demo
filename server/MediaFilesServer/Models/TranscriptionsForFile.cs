using ActIntelligenceService.Domain.Models.InsightProviders;

namespace FRServer.Models
{
    public class TranscriptionsForFile
    {
        public string FileName { get; set; }
        public List<Transcript>? Transcripts { get; set; }
    }
}
