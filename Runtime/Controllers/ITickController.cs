using System;

namespace DTech.TickSystem
{
    public interface ITickController : IDisposable
    {
        bool TryAdd(object owner, Action<float> tick, int order);
        bool TryRemove(Action<float> tick);
    }
}