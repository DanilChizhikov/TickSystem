using System;

namespace MbsCore.TickSystem.Infrastructure
{
    public interface ITickService
    {
        IDisposable AddTick(IBaseTickable value);
    }
}
