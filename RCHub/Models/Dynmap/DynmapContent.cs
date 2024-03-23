namespace RCHub.Models.Dynmap
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public partial class DynmapContent
    {
        [JsonProperty("sets")]
        public Dictionary<string, Set> Sets { get; set; } = new Dictionary<string, Set>();

        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }
    }

    public partial class Set
    {
        [JsonProperty("hide")]
        public bool Hide { get; set; }

        [JsonProperty("label")]
        public string? Label { get; set; }

        [JsonProperty("markers")]
        public Dictionary<string, Marker>? Markers { get; set; }

        [JsonProperty("areas")]
        public Dictionary<string, Area>? Areas { get; set; }

    }

    public class Marker
    {
        [JsonProperty("markup")]
        public bool Markup { get; set; }

        [JsonProperty("x")]
        public double X { get; set; }

        [JsonProperty("icon")]
        public string? Icon { get; set; }

        [JsonProperty("y")]
        public double Y { get; set; }

        [JsonProperty("dim")]
        public string? Dim { get; set; }

        [JsonProperty("z")]
        public double Z { get; set; }

        [JsonProperty("label")]
        public string? Label { get; set; }
        [JsonProperty("desc")]
        public string? Desc { get; set; }
    }

    public class Area
    {
        [JsonProperty("fillcolor")]
        public string? Fillcolor { get; set; }

        [JsonProperty("ytop")]
        public double Ytop { get; set; }

        [JsonProperty("color")]
        public string? Color { get; set; }

        [JsonProperty("markup")]
        public bool Markup { get; set; }

        [JsonProperty("x")]
        public List<double>? X { get; set; }

        [JsonProperty("weight")]
        public int Weight { get; set; }

        [JsonProperty("z")]
        public List<double>? Z { get; set; }

        [JsonProperty("ybottom")]
        public double Ybottom { get; set; }

        [JsonProperty("label")]
        public string? Label { get; set; }

        [JsonProperty("opacity")]
        public double Opacity { get; set; }

        [JsonProperty("fillopacity")]
        public double Fillopacity { get; set; }

        [JsonProperty("desc")]
        public string? Desc { get; set; }
    }
}
