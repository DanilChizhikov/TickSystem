using System;

namespace MbsCore.TickSystem
{
    public interface ITickService
    {
        IDisposable AddTick(IBaseTickable value);
    }
}
