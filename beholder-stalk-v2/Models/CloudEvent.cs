namespace beholder_stalk_v2.Models
{
  using System;
  using System.Collections.Generic;
  using System.Text.Json.Serialization;

  public record CloudEvent : ICloudEvent
  {
    public CloudEvent()
    {
      Id = Guid.NewGuid().ToString();
      Time = DateTime.Now;
      DataContentType = "application/json";
      SpecVersion = "1.0";
      ExtensionAttributes = new Dictionary<string, object>();
    }

    [JsonPropertyName("datacontenttype")]
    public string DataContentType { get; init; }

    [JsonPropertyName("dataschema")]
    public string DataSchema { get; init; }

    [JsonPropertyName("id")]
    public string Id { get; init; }

    [JsonPropertyName("source")]
    public string Source { get; init; }

    [JsonPropertyName("specversion")]
    public string SpecVersion { get; init; }

    [JsonPropertyName("subject")]
    public string Subject { get; init; }

    [JsonPropertyName("time")]
    public DateTime Time { get; init; }

    [JsonPropertyName("type")]
    public string Type { get; init; }

    [JsonExtensionData]
    public IDictionary<string, object> ExtensionAttributes { get; init; }
  }

  public record CloudEvent<T> : CloudEvent, ICloudEvent<T>
  {
    [JsonPropertyName("data")]
    public T Data { get; set; }
  }
}