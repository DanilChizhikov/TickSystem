using System;
using UnityEngine;

namespace MbsCore.TickSystem
{
    public sealed class TickableEntity : IFixTickable, ITickable, ILateTickable, IDisposable
    {
        private readonly IDisposable _fixTickDisposable;
        private readonly IDisposable _tickDisposable;
        private readonly IDisposable _lateTickDisposable;
        
        public TickableEntity(ITickService tickService)
        {
            _fixTickDisposable = tickService.AddFixTick(this);
            _tickDisposable = tickService.AddTick(this);
            _lateTickDisposable = tickService.AddLateTick(this);
        }
        
        public void FixTick(float deltaTime)
        {
            Debug.Log($"{nameof(FixTick)}: {deltaTime}");
        }

        public void Tick(float deltaTime)
        {
            Debug.Log($"{nameof(Tick)}: {deltaTime}");
        }

        public void LateTick(float deltaTime)
        {
            Debug.Log($"{nameof(LateTick)}: {deltaTime}");
        }

        public void Dispose()
        {
            _fixTickDisposable?.Dispose();
            _tickDisposable?.Dispose();
            _lateTickDisposable?.Dispose();
        }
    }
}