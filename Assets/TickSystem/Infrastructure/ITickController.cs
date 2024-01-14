using System;

namespace MbsCore.TickSystem
{
    public interface ITickController : IDisposable
    {
        bool TryAdd(Action<float> tick, int order);
        bool TryRemove(Action<float> tick);
    }
}