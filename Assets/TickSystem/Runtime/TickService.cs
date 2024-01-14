using System;
using System.Collections.Generic;

namespace MbsCore.TickSystem
{
    public sealed class TickService : ITickService, IDisposable
    {
        private readonly ITickController _fixTickController;
        private readonly ITickController _tickController;
        private readonly ITickController _lateTickController;
        private readonly List<IDisposable> _fixTickDisposables;
        private readonly List<IDisposable> _tickDisposables;
        private readonly List<IDisposable> _lateTickDisposables;

        public TickService()
        {
            _fixTickController = new FixedUpdateTickController();
            _tickController = new UpdateTickController();
            _lateTickController = new LateUpdateTickController();
        }
        
        public IDisposable AddFixTick(IFixTickable value, int order = Int32.MaxValue)
        {
            if (!_fixTickController.TryAdd(value.FixTick, order))
            {
                return null;
            }

            IDisposable handler = CreateTickDisposable(value.FixTick, RemoveFixTickable);
            _fixTickDisposables.Add(handler);
            return handler;
        }

        public IDisposable AddTick(ITickable value, int order = Int32.MaxValue)
        {
            if (!_tickController.TryAdd(value.Tick, order))
            {
                return null;
            }

            IDisposable handler = CreateTickDisposable(value.Tick, RemoveUpdateTickable);
            _tickDisposables.Add(handler);
            return handler;
        }

        public IDisposable AddLateTick(ILateTickable value, int order = Int32.MaxValue)
        {
            if (!_lateTickController.TryAdd(value.LateTick, order))
            {
                return null;
            }

            IDisposable handler = CreateTickDisposable(value.LateTick, RemoveLateTickable);
            _lateTickDisposables.Add(handler);
            return handler;
        }
        
        public void Dispose()
        {
            _fixTickController.Dispose();
            _tickController.Dispose();
            _lateTickController.Dispose();
            _fixTickDisposables.Clear();
            _tickDisposables.Clear();
            _lateTickDisposables.Clear();
        }

        private IDisposable CreateTickDisposable(Action<float> tick, Action<TickableHandler> disposeCallback)
        {
            var handler = new TickableHandler(tick);
            handler.OnDisposed += disposeCallback;
            return handler;
        }

        private void RemoveTickable(TickableHandler handler, ITickController controller, List<IDisposable> disposables)
        {
            controller.TryRemove(handler.TickAction);
            disposables.Remove(handler);
        }

        private void RemoveFixTickable(TickableHandler handler)
        {
            RemoveTickable(handler, _fixTickController, _fixTickDisposables);
        }

        private void RemoveUpdateTickable(TickableHandler handler)
        {
            RemoveTickable(handler, _tickController, _tickDisposables);
        }

        private void RemoveLateTickable(TickableHandler handler)
        {
            RemoveTickable(handler, _lateTickController, _lateTickDisposables);
        }
    }
}