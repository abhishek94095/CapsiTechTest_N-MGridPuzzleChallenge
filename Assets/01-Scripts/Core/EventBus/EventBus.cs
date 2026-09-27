using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AV.Framework.Core.Logging;

namespace AV.Framework.Core.EventBus
{
    public sealed class EventBus : IEventBus
    {
        private readonly Dictionary<Type, List<Delegate>> syncSubscribers = new();
        private readonly Dictionary<Type, List<Delegate>> asyncSubscribers = new();
        private readonly ILogger logger;

        public EventBus(ILogger logger)
        {
            this.logger = logger;
        }

        public IDisposable Subscribe<T>(Action<T> callback)
        {
            Type eventType = typeof(T);

            if (!syncSubscribers.TryGetValue(eventType, out List<Delegate> subscribers))
            {
                subscribers = new List<Delegate>();
                syncSubscribers[eventType] = subscribers;
            }

            if (!subscribers.Contains(callback))
            {
                subscribers.Add(callback);
            }

            return new EventSubscription(() => Unsubscribe(callback));
        }

        public void Unsubscribe<T>(Action<T> callback)
        {
            Type eventType = typeof(T);

            if (!syncSubscribers.TryGetValue(eventType, out List<Delegate> subscribers))
            {
                return;
            }

            subscribers.Remove(callback);

            if (subscribers.Count == 0)
            {
                syncSubscribers.Remove(eventType);
            }
        }

        public void Publish<T>(T eventData)
        {
            Type eventType = typeof(T);

            if (!syncSubscribers.TryGetValue(eventType, out List<Delegate> subscribers))
            {
                return;
            }

            Delegate[] subscribersSnapshot = subscribers.ToArray();

            foreach (Delegate subscriber in subscribersSnapshot)
            {
                try
                {
                    ((Action<T>)subscriber).Invoke(eventData);
                }
                catch (Exception exception)
                {
                    logger.LogError($"Exception while publishing event '{eventType.Name}'.\n{exception}");
                }
            }
        }

        public IDisposable SubscribeAsync<T>(Func<T, Task> callback)
        {
            Type eventType = typeof(T);

            if (!asyncSubscribers.TryGetValue(eventType, out List<Delegate> subscribers))
            {
                subscribers = new List<Delegate>();
                asyncSubscribers[eventType] = subscribers;
            }

            if (!subscribers.Contains(callback))
            {
                subscribers.Add(callback);
            }

            return new EventSubscription(() => UnsubscribeAsync(callback));
        }

        public void UnsubscribeAsync<T>(Func<T, Task> callback)
        {
            Type eventType = typeof(T);

            if (!asyncSubscribers.TryGetValue(eventType, out List<Delegate> subscribers))
            {
                return;
            }

            subscribers.Remove(callback);

            if (subscribers.Count == 0)
            {
                asyncSubscribers.Remove(eventType);
            }
        }

        public async Task PublishAsync<T>(T eventData)
        {
            Type eventType = typeof(T);

            if (!asyncSubscribers.TryGetValue(eventType, out List<Delegate> subscribers))
            {
                return;
            }

            Delegate[] subscribersSnapshot = subscribers.ToArray();

            foreach (Delegate subscriber in subscribersSnapshot)
            {
                try
                {
                    await ((Func<T, Task>)subscriber).Invoke(eventData);
                }
                catch (Exception exception)
                {
                    logger.LogError($"Exception while publishing async event '{eventType.Name}'.\n{exception}");
                }
            }
        }
    }
}
