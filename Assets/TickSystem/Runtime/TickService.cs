using System;
using System.Collections.Generic;

namespace MbsCore.TickSystem
{
    public sealed class TickService : ITickService, IDisposable
    {
        private readonly TickKernel _tickKernel;
        private readonly Dictionary<IBaseTickable, HashSet<ITickController>> _tickableMap;
        private readonly Dictionary<IBaseTickable, TickableHandler> _handlersMap;

        public TickService()
        {
            _tickKernel = new TickKernel();
            _tickableMap = new Dictionary<IBaseTickable, HashSet<ITickController>>();
            _handlersMap = new Dictionary<IBaseTickable, TickableHandler>();
        }
        
        public IDisposable AddTick(IBaseTickable value)
        {
            bool wasAdded = false;
            if (!_handlersMap.TryGetValue(value, out TickableHandler handler))
            {
                if (!_tickableMap.TryGetValue(value, out HashSet<ITickController> controllers))
                {
                    controllers = new HashSet<ITickController>();
                    _tickableMap.Add(value, controllers);
                }
                
                wasAdded |= TryAddTickable(value, _tickKernel.TickController, controllers);
                wasAdded |= TryAddTickable(value, _tickKernel.FixTickController, controllers);
                wasAdded |= TryAddTickable(value, _tickKernel.LateTickController, controllers);
            }

            if (wasAdded)
            {
                handler = new TickableHandler(value);
                handler.OnDisposed += TickableDisposedCallback;
                _handlersMap.Add(value, handler);
            }

            return handler;
        }

        public void Dispose()
        {
            _tickKernel.Dispose();
            var handlers = new HashSet<TickableHandler>(_handlersMap.Values);
            foreach (var handler in handlers)
            {
                handler.Dispose();
            }
        }

        private bool TryAddTickable(IBaseTickable tickable, ITickController controller, HashSet<ITickController> controllers)
        {
            if (!controller.Add(tickable))
            {
                return false;
            }

            return controllers.Add(controller);
        }
        
        private void TickableDisposedCallback(IBaseTickable tickable)
        {
            if (_tickableMap.TryGetValue(tickable, out HashSet<ITickController> controllers))
            {
                foreach (var controller in controllers)
                {
                    controller.Remove(tickable);
                }
                
                _tickableMap.Remove(tickable);
                _handlersMap.Remove(tickable);
            }
        }
    }
}