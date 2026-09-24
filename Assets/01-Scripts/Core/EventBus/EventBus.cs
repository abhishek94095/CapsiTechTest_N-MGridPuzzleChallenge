using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AV.Framework.Core.Logging;

namespace AV.Framework.Core.EventBus
{
    public sealed class EventBus : IEventBus
    {
        private readonly Dictionary<Type, List<Delegate>> _syncSubscribers = new();
        private readonly Dictionary<Type, List<Delegate>> _asyncSubscribers = new();
        private readonly ILogger _logger;

        public EventBus(ILogger logger)
        {
            _logger = logger;
        }

        public IDisposable Subscribe<T>(Action<T> callback)
        {
            Type eventType = typeof(T);

            if (!_syncSubscribers.TryGetValue(eventType, out List<Delegate> subscribers))
            {
                subscribers = new List<Delegate>();
                _syncSubscribers[eventType] = subscribers;
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

            if (!_syncSubscribers.TryGetValue(eventType, out List<Delegate> subscribers))
            {
                return;
            }

            subscribers.Remove(callback);

            if (subscribers.Count == 0)
            {
                _syncSubscribers.Remove(eventType);
            }
        }

        public void Publish<T>(T eventData)
        {
            Type eventType = typeof(T);

            if (!_syncSubscribers.TryGetValue(eventType, out List<Delegate> subscribers))
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
                    _logger.LogError($"Exception while publishing event '{eventType.Name}'.\n{exception}");
                }
            }
        }

        public IDisposable SubscribeAsync<T>(Func<T, Task> callback)
        {
            Type eventType = typeof(T);

            if (!_asyncSubscribers.TryGetValue(eventType, out List<Delegate> subscribers))
            {
                subscribers = new List<Delegate>();
                _asyncSubscribers[eventType] = subscribers;
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

            if (!_asyncSubscribers.TryGetValue(eventType, out List<Delegate> subscribers))
            {
                return;
            }

            subscribers.Remove(callback);

            if (subscribers.Count == 0)
            {
                _asyncSubscribers.Remove(eventType);
            }
        }

        public async Task PublishAsync<T>(T eventData)
        {
            Type eventType = typeof(T);

            if (!_asyncSubscribers.TryGetValue(eventType, out List<Delegate> subscribers))
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
                    _logger.LogError($"Exception while publishing async event '{eventType.Name}'.\n{exception}");
                }
            }
        }
    }
}
