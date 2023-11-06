namespace FRServer.Models
{
    class SubjectDetailsWithNameAndScores
    {
        public string Name { get; set; }
        public List<TimeRangeAndScore> TimeRangeAndScores { get; set; }
    }
}
