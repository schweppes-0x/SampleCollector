using System.Text.Json.Serialization;

namespace SampleCollector.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum FileType
    {
        JSON,
        CSV,
        XML,
        HTML
    }
}