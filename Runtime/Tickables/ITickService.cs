using System;

namespace DTech.TickSystem
{
    public interface ITickService
    {
        IDisposable AddFixTick(IFixTickable value);
        IDisposable AddTick(ITickable value);
        IDisposable AddLateTick(ILateTickable value);
    }
}
