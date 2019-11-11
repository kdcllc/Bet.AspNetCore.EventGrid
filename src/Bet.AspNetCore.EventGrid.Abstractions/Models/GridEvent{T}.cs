using System;

namespace Bet.AspNetCore.EventGrid.Abstractions.Models
{
    /// <summary>
    /// Azure Event Grid Event type.
    /// https://docs.microsoft.com/en-us/azure/event-grid/event-schema.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GridEvent<T> where T : class, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridEvent{T}"/> class.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="eventType"></param>
        public GridEvent(T data, string? eventType = null) : base()
        {
            Data = data;
            EventType = eventType ?? typeof(T).Name;
        }

        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string EventType { get; set; } = typeof(T).Name;

        public string Subject { get; set; } = string.Empty;

        public DateTimeOffset EventTime { get; set; } = DateTimeOffset.UtcNow;

        public T Data { get; set; } = new T();

        public string Topic { get; set; } = string.Empty;
    }
}
