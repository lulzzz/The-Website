using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SA.Web.Shared.Data.WebSockets
{
    public class RoadmapData
    {
        [JsonPropertyName("Cards")]
        public List<RoadmapCard> Cards { get; set; }
    }

    public class RoadmapCard
    {
        [JsonPropertyName("MajorVersion")]
        public int MajorVersion { get; set; }
        [JsonPropertyName("MinorVersion")]
        public int MinorVersion { get; set; }
        [JsonPropertyName("Description")]
        public string Description { get; set; }
        [JsonPropertyName("VersionFeatures")]
        public List<RoadmapFeature> VersionFeatures { get; set; }
        [JsonPropertyName("Patches")]
        public List<RoadmapCardChangelog> Patches { get; set; }
    }

    public class RoadmapCardVersions
    {
        [JsonPropertyName("Versions")]
        public List<string> Versions { get; set; }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum RoadmapCardCategory : int
    {
        [EnumMember(Value = "Characters")]
        Characters,
        [EnumMember(Value = "Locations")]
        Locations,
        [EnumMember(Value = "AI")]
        AI,
        [EnumMember(Value = "Gameplay")]
        Gameplay,
        [EnumMember(Value = "ShipsAndVehicles")]
        ShipsAndVehicles,
        [EnumMember(Value = "WeaponsAndItems")]
        WeaponsAndItems,
        [EnumMember(Value = "CoreTech")]
        CoreTech
    }

    public class RoadmapCardChangelog
    {
        [JsonPropertyName("PatchVersion")]
        public int PatchVersion { get; set; }
        [JsonPropertyName("VersionString")]
        public string VersionString { get; set; }
        [JsonPropertyName("PUReleaseDate")]
        public DateTime? PUReleaseDate { get; set; }
        [JsonPropertyName("EvocatiTestingDate")]
        public DateTime? EvocatiTestingDate { get; set; }
        [JsonPropertyName("PTUTestingStartDate")]
        public DateTime? PTUTestingStartDate { get; set; }
        [JsonPropertyName("SpectrumPatchNotes")]
        public string SpectrumPatchNotes { get; set; }
    }

    public class RoadmapFeature
    {
        [JsonPropertyName("Title")]
        public string Title { get; set; }
        [JsonPropertyName("Category")]
        public RoadmapCardCategory Category { get; set; }
        [JsonPropertyName("Status")]
        [JsonConverter(typeof(DictionaryDateTimeRoadmapFeatureStatusConverter))]
        public Dictionary<DateTime, RoadmapFeatureStatus> Status { get; set; }
        [JsonPropertyName("TaskCount")]
        [JsonConverter(typeof(DictionaryDateTimeIntConverter))]
        public Dictionary<DateTime, int> TaskCount { get; set; }
        [JsonPropertyName("TasksCompleted")]
        [JsonConverter(typeof(DictionaryDateTimeIntConverter))]
        public Dictionary<DateTime, int> TasksCompleted { get; set; }
        [JsonPropertyName("Description")]
        public string Description { get; set; }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))] 
    public enum RoadmapFeatureStatus : int
    {
        [EnumMember(Value = "Scheduled")]
        Scheduled,
        [EnumMember(Value = "InDevelopment")]
        InDevelopment,
        [EnumMember(Value = "Polishing")]
        Polishing,
        [EnumMember(Value = "Released")]
        Released
    }

    // Json Converters

    public class DictionaryDateTimeIntConverter : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(Dictionary<DateTime, int>);
        }

        public override JsonConverter CreateConverter(Type type, JsonSerializerOptions options)
        {
            return (JsonConverter)Activator.CreateInstance(typeof(DictionaryDateTimeIntConverterInner),
                    BindingFlags.Instance | BindingFlags.Public,
                    binder: null,
                    args: new object[] { options },
                    culture: null);
        }

        private class DictionaryDateTimeIntConverterInner : JsonConverter<Dictionary<DateTime, int>>
        {
            public DictionaryDateTimeIntConverterInner(JsonSerializerOptions options) { }

            public override Dictionary<DateTime, int> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                Dictionary<DateTime, int> dictionary = new Dictionary<DateTime, int>();
                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndObject) return dictionary;
                    if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException();
                    string propName = reader.GetString();
                    if (!DateTime.TryParse(propName, out DateTime key)) throw new JsonException("Unable to convert " + key);
                    reader.Read();
                    int v = JsonSerializer.Deserialize<int>(ref reader, options);
                    dictionary.Add(key, v);
                }
                throw new JsonException();
            }

            public override void Write(Utf8JsonWriter writer, Dictionary<DateTime, int> dictionary, JsonSerializerOptions options)
            {
                writer.WriteStartObject();
                foreach (KeyValuePair<DateTime, int> kvp in dictionary)
                {
                    writer.WritePropertyName(kvp.Key.ToString());
                    JsonSerializer.Serialize(writer, kvp.Value, options);
                }
                writer.WriteEndObject();
            }
        }
    }

    public class DictionaryDateTimeRoadmapFeatureStatusConverter : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(Dictionary<DateTime, RoadmapFeatureStatus>);
        }

        public override JsonConverter CreateConverter(Type type, JsonSerializerOptions options)
        {
            return (JsonConverter)Activator.CreateInstance(
                typeof(DictionaryDateTimeRoadmapFeatureStatusConverterInner),
                    BindingFlags.Instance | BindingFlags.Public,
                    binder: null,
                    args: new object[] { options },
                    culture: null);
        }

        private class DictionaryDateTimeRoadmapFeatureStatusConverterInner : JsonConverter<Dictionary<DateTime, RoadmapFeatureStatus>>
        {
            public DictionaryDateTimeRoadmapFeatureStatusConverterInner(JsonSerializerOptions options) { }

            public override Dictionary<DateTime, RoadmapFeatureStatus> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                Dictionary<DateTime, RoadmapFeatureStatus> dictionary = new Dictionary<DateTime, RoadmapFeatureStatus>();
                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndObject) return dictionary;
                    if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException();

                    string propName = reader.GetString();
                    if (!DateTime.TryParse(propName, out DateTime key)) throw new JsonException("Unable to convert " + key);
                    reader.Read();
                    dictionary.Add(key, (RoadmapFeatureStatus)Enum.Parse(typeof(RoadmapFeatureStatus), reader.GetString()));
                }
                throw new JsonException();
            }

            public override void Write(Utf8JsonWriter writer, Dictionary<DateTime, RoadmapFeatureStatus> dictionary, JsonSerializerOptions options)
            {
                writer.WriteStartObject();
                foreach (KeyValuePair<DateTime, RoadmapFeatureStatus> kvp in dictionary)
                {
                    writer.WritePropertyName(kvp.Key.ToString());
                    JsonSerializer.Serialize(writer, kvp.Value, options);
                }
                writer.WriteEndObject();
            }
        }
    }
}
