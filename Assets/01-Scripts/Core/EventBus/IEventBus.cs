using System;
using System.Threading.Tasks;

namespace AV.Framework.Core.EventBus
{
    public interface IEventBus
    {
        IDisposable Subscribe<T>(Action<T> callback);

        void Unsubscribe<T>(Action<T> callback);

        void Publish<T>(T eventData);

        IDisposable SubscribeAsync<T>(Func<T, Task> callback);

        void UnsubscribeAsync<T>(Func<T, Task> callback);

        Task PublishAsync<T>(T eventData);
    }
}
