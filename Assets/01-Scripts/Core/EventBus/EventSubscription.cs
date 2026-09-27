using System;

namespace AV.Framework.Core.EventBus
{
    public sealed class EventSubscription : IDisposable
    {
        private Action disposeAction;

        public EventSubscription(Action disposeAction)
        {
            this.disposeAction = disposeAction;
        }

        public void Dispose()
        {
            disposeAction?.Invoke();
            disposeAction = null;
        }
    }
}
