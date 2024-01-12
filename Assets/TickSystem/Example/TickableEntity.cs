using System;
using UnityEngine;

namespace MbsCore.TickSystem
{
    public sealed class TickableEntity : IFixTickable, ITickable, ILateTickable, IDisposable
    {
        private readonly IDisposable _tickDisposable;
        
        public TickableEntity(ITickService tickService)
        {
            _tickDisposable = tickService.AddTick(this);
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
            _tickDisposable?.Dispose();
        }
    }
}