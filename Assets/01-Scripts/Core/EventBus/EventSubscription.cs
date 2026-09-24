using System;

namespace AV.Framework.Core.EventBus
{
    public sealed class EventSubscription : IDisposable
    {
        private Action _disposeAction;

        public EventSubscription(Action disposeAction)
        {
            _disposeAction = disposeAction;
        }

        public void Dispose()
        {
            _disposeAction?.Invoke();
            _disposeAction = null;
        }
    }
}
