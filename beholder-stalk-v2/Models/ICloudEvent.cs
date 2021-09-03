namespace beholder_stalk_v2.Models
{
  using System;
  using System.Collections.Generic;

  public interface ICloudEvent
  {
    /// <summary>
    /// Content type of data value.
    /// </summary>
    /// <example>
    /// "application/json"
    /// </example>
    string DataContentType
    {
      get;
    }

    /// <summary>
    /// Identifies the schema that data adheres to.
    /// </summary>
    string DataSchema
    {
      get;
    }

    /// <summary>
    /// An identifier for the event. The combination of id and source must be unique for each distinct event.
    /// </summary>
    /// <example>
    /// "1234-1234-1234"
    /// </example>
    string Id
    {
      get;
    }

    /// <summary>
    /// Identifies the context in which an event happened. The combination of id and source must be unique for each distinct event.
    /// </summary>
    /// <example>
    /// "/mycontext"
    /// </example>
    string Source
    {
      get;
    }

    string SpecVersion
    {
      get;
    }

    /// <summary>
    /// This describes the subject of the event in the context of the event producer (identified by source).
    /// </summary>
    /// <example>
    /// "larger-context"
    /// </example>
    string Subject
    {
      get;
    }

    /// <summary>
    /// The time the event was generated.
    /// </summary>
    /// <example>
    /// "2018-04-05T17:31:00Z"
    /// </example>
    DateTime Time
    {
      get;
    }

    /// <summary>
    /// Type of event related to the originating occurrence.
    /// </summary>
    /// <example>
    /// "com.example.someevent"
    /// </example>
    string Type
    {
      get;
    }

    /// <summary>
    /// Additional context attributes for the event. The Cloud Event specification refers to these as "extension attributes".
    /// </summary>
    IDictionary<string, object> ExtensionAttributes
    {
      get;
    }
  }

  public interface ICloudEvent<T> : ICloudEvent
  {
    /// <summary>
    /// Event data specific to the event type. 
    /// </summary>
    T Data
    {
      get;
    }
  }
}