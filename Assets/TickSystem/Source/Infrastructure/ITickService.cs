using System;

namespace DTech.TickSystem
{
    public interface ITickService
    {
        IDisposable AddFixTick(IFixTickable value, int order = int.MaxValue);
        IDisposable AddTick(ITickable value, int order = int.MaxValue);
        IDisposable AddLateTick(ILateTickable value, int order = int.MaxValue);
    }
}
