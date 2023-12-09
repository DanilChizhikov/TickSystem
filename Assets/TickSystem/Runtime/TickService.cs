using System;
using System.Collections.Generic;
using MbsCore.TickSystem.Infrastructure;
using MbsCore.TickSystem.Runtime.Controllers;
using UnityEngine;

namespace MbsCore.TickSystem.Runtime
{
    public sealed class TickService : ITickService, IDisposable
    {
        private readonly Dictionary<IBaseTickable, HashSet<ITickController>> _tickableMap;
        private readonly Dictionary<IBaseTickable, TickableHandler> _handlersMap;
        private readonly ITickController _fixController;
        private readonly ITickController _defaultController;
        private readonly ITickController _lateController;

        private TickMonoKernel _kernel;

        public TickService()
        {
            _tickableMap = new Dictionary<IBaseTickable, HashSet<ITickController>>();
            _handlersMap = new Dictionary<IBaseTickable, TickableHandler>();
            _fixController = new FixTickController();
            _defaultController = new DefaultTickController();
            _lateController = new LateTickController();
            CreateTickKernel();
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
                
                wasAdded |= TryAddTickable(value, _defaultController, controllers);
                wasAdded |= TryAddTickable(value, _fixController, controllers);
                wasAdded |= TryAddTickable(value, _lateController, controllers);
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
            _kernel.Dispose();
            var handlers = new HashSet<TickableHandler>(_handlersMap.Values);
            foreach (var handler in handlers)
            {
                handler.Dispose();
            }
        }

        private void CreateTickKernel()
        {
            _kernel = new GameObject(nameof(TickMonoKernel)).AddComponent<TickMonoKernel>();
            var controllers = new Dictionary<TickMonoKernel.TickType, ITickController>(3);
            controllers.Add(TickMonoKernel.TickType.Default, _defaultController);
            controllers.Add(TickMonoKernel.TickType.Fix, _fixController);
            controllers.Add(TickMonoKernel.TickType.Late, _lateController);
            _kernel.Initialize(controllers);
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