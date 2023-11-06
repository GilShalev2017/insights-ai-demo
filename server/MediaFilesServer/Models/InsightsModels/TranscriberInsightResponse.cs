namespace ActIntelligenceService.Domain.Models.InsightProviders
{
    public class Segment
    {
        public int id { get; set; }
        public double start { get; set; }
        public double end { get; set; }
        public string text { get; set; }
    }
    public class Root
    {
        public string task { get; set; }
        public string language { get; set; }
        public double duration { get; set; }
        public List<Segment> segments { get; set; }
    }

    public class Transcript
    {
        public string Text { get; set; }
        public int StartInSeconds { get; set; }
        public int EndInSeconds { get; set; }
    }

    public class TranscriberInsightResponse
    {
        public List<Transcript>? Transcripts { get; set; }
    }
}
