using System.Reflection;
using static ActInfra.GlobalEnums;

namespace ActIntelligenceService.Domain.Models
{
    public class InsightType : ActEnums
    {
        static public readonly InsightType None = new("None");
        static public readonly InsightType Transcription = new("Transcription");
        static public readonly InsightType Translation = new("Translation");
        static public readonly InsightType FaceRecognition = new("FaceRecognition");
        static public readonly InsightType Summary= new("Summary");
        static public readonly InsightType KeyPoints = new("KeyPoints");
        static public readonly InsightType ActionItems = new("ActionItems");
        static public readonly InsightType Sentiment = new("Sentiment");

        public InsightType(string value) : base(value) { }

        public static InsightType Parse(string valuetoparse)
        {
            Type enumType = typeof(InsightType);
            List<InsightType> values = enumType.GetFields(BindingFlags.Public | BindingFlags.Static)
                                .Where(f => f.FieldType == typeof(InsightType))
                                .Select(f => f.GetValue(null) as InsightType)
                                .ToList()!;

            foreach(InsightType e in values)
            {
                if(e.ToString().ToLower().Trim() == valuetoparse.ToLower().Trim())
                    return e;
            }

            return InsightType.None;
        }
    }
}
