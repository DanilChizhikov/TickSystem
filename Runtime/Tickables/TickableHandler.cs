using System;

namespace DTech.TickSystem
{
    internal sealed class TickableHandler : IDisposable
    {
        public event Action<TickableHandler> OnDisposed; 
        
        public Action<float> TickAction { get; }

        public TickableHandler(Action<float> tickAction)
        {
            TickAction = tickAction;
        }
        
        public void Dispose()
        {
            OnDisposed?.Invoke(this);
        }
    }
}