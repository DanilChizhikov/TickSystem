using System;

namespace MbsCore.TickSystem
{
    internal sealed class TickableHandler : IDisposable
    {
        public event Action<IBaseTickable> OnDisposed;
        
        public IBaseTickable Tickable { get; }

        public TickableHandler(IBaseTickable tickable)
        {
            Tickable = tickable;
        }
        
        public void Dispose()
        {
            OnDisposed?.Invoke(Tickable);
        }
    }
}