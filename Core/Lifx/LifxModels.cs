namespace Lights.Core.Lifx
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class Light
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("uuid")]
        public Guid Uuid { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("connected")]
        public bool Connected { get; set; }

        [JsonProperty("power")]
        public string Power { get; set; }

        [JsonProperty("color")]
        public Color Color { get; set; }

        [JsonProperty("brightness")]
        public double Brightness { get; set; }

        [JsonProperty("effect")]
        public string Effect { get; set; }

        [JsonProperty("group")]
        public Group Group { get; set; }

        [JsonProperty("location")]
        public Group Location { get; set; }

        [JsonProperty("product")]
        public Product Product { get; set; }

        [JsonProperty("last_seen")]
        public DateTimeOffset? LastSeen { get; set; }

        [JsonProperty("seconds_since_seen")]
        public long SecondsSinceSeen { get; set; }
    }

    public partial class Color
    {
        [JsonProperty("hue")]
        public double Hue { get; set; }

        [JsonProperty("saturation")]
        public float Saturation { get; set; }

        [JsonProperty("kelvin")]
        public long Kelvin { get; set; }
    }

    public partial class Group
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public partial class Product
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("identifier")]
        public string Identifier { get; set; }

        [JsonProperty("company")]
        public string Company { get; set; }

        [JsonProperty("capabilities")]
        public Capabilities Capabilities { get; set; }
    }

    public partial class Capabilities
    {
        [JsonProperty("has_color")]
        public bool HasColor { get; set; }

        [JsonProperty("has_variable_color_temp")]
        public bool HasVariableColorTemp { get; set; }

        [JsonProperty("has_ir")]
        public bool HasIr { get; set; }

        [JsonProperty("has_chain")]
        public bool HasChain { get; set; }

        [JsonProperty("has_multizone")]
        public bool HasMultizone { get; set; }

        [JsonProperty("min_kelvin")]
        public long MinKelvin { get; set; }

        [JsonProperty("max_kelvin")]
        public long MaxKelvin { get; set; }
    }

    public partial class States
    {
        [JsonProperty("states")]
        public List<State> StateList { get; set; }

        [JsonProperty("fast")]
        public bool Fast { get; set; }
    }

    public partial class State
    {
        [JsonProperty("selector")]
        public string Selector { get; set; }

        [JsonProperty("color", NullValueHandling = NullValueHandling.Ignore)]
        public string Color { get; set; }
    }
}
