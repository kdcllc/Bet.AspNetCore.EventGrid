using System;

using Newtonsoft.Json;

namespace Bet.AspNetCore.EvenGrid.Models
{
    /// <summary>
    /// Azure Cloud Event.
    /// https://docs.microsoft.com/en-us/azure/event-grid/cloudevents-schema#cloudevent-schema.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CloudEvent<T> where T : class, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CloudEvent{T}"/> class.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="eventType"></param>
        public CloudEvent(T data, string? eventType = null)
        {
            Data = data;
            EventType = eventType ?? typeof(T).Name;
        }

        [JsonProperty("eventID")]
        public string EventId { get; set; } = Guid.NewGuid().ToString();

        [JsonProperty("cloudEventsVersion")]
        public string CloudEventVersion { get; set; } = "0.1";

        [JsonProperty("eventType")]
        public string EventType { get; set; } = typeof(T).Name;

        [JsonProperty("eventTypeVersion")]
        public string EventTypeVersion { get; set; } = "1.0";

        [JsonProperty("source")]
        public string Source { get; set; } = "k8s-cluster";

        [JsonProperty("eventTime")]
        public string EventTime { get; set; } = DateTimeOffset.UtcNow.ToString("o");

        [JsonProperty("data")]
        public T Data { get; set; } = new T();
    }
}
