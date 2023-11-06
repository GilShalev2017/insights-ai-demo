namespace ActIntelligenceService.Domain.Models
{
    public class InsightResult
    {

        public class SearchDM
        {
            public int Offset { get; set; }

            public string Content { get; set; } = null!;
        }

        public List<SearchDM>? SearchData { get; set; }

        public string? BulkData { get; set; } // json string 

        public string? AIProviderId { get; set; }

        public double Cost { get; set; } = 0;

    }
}
