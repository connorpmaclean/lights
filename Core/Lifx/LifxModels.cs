namespace Lights.Core.Lifx
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public partial class Light
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("uuid")]
        public Guid Uuid { get; set; }

        [JsonPropertyName("label")]
        public string Label { get; set; }

        [JsonPropertyName("connected")]
        public bool Connected { get; set; }

        [JsonPropertyName("power")]
        public string Power { get; set; }

        [JsonPropertyName("color")]
        public Color Color { get; set; }

        [JsonPropertyName("brightness")]
        public double Brightness { get; set; }

        [JsonPropertyName("effect")]
        public string Effect { get; set; }

        [JsonPropertyName("group")]
        public Group Group { get; set; }

        [JsonPropertyName("location")]
        public Group Location { get; set; }

        [JsonPropertyName("product")]
        public Product Product { get; set; }

        [JsonPropertyName("last_seen")]
        public DateTimeOffset? LastSeen { get; set; }

        [JsonPropertyName("seconds_since_seen")]
        public long SecondsSinceSeen { get; set; }
    }

    public partial class Color
    {
        [JsonPropertyName("hue")]
        public double Hue { get; set; }

        [JsonPropertyName("saturation")]
        public float Saturation { get; set; }

        [JsonPropertyName("kelvin")]
        public long Kelvin { get; set; }
    }

    public partial class Group
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    public partial class Product
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("identifier")]
        public string Identifier { get; set; }

        [JsonPropertyName("company")]
        public string Company { get; set; }

        [JsonPropertyName("capabilities")]
        public Capabilities Capabilities { get; set; }
    }

    public partial class Capabilities
    {
        [JsonPropertyName("has_color")]
        public bool HasColor { get; set; }

        [JsonPropertyName("has_variable_color_temp")]
        public bool HasVariableColorTemp { get; set; }

        [JsonPropertyName("has_ir")]
        public bool HasIr { get; set; }

        [JsonPropertyName("has_chain")]
        public bool HasChain { get; set; }

        [JsonPropertyName("has_multizone")]
        public bool HasMultizone { get; set; }

        [JsonPropertyName("min_kelvin")]
        public long MinKelvin { get; set; }

        [JsonPropertyName("max_kelvin")]
        public long MaxKelvin { get; set; }
    }

    public partial class States
    {
        [JsonPropertyName("states")]
        public List<State> StateList { get; set; }

        [JsonPropertyName("fast")]
        public bool Fast { get; set; }
    }

    public partial class State
    {
        [JsonPropertyName("selector")]
        public string Selector { get; set; }

        [JsonPropertyName("color")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Color { get; set; }
    }
}
