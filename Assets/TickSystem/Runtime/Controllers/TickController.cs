using System;
using System.Collections.Generic;

namespace MbsCore.TickSystem
{
    internal abstract class TickController<T> : ITickController where T : IBaseTickable
    {
        private class TickableComparer : IComparer<T>
        {
            public int Compare(T x, T y)
            {
                return x.TickOrder.CompareTo(y.TickOrder);
            }
        }
        
        private readonly List<T> _tickables;
        private readonly Queue<T> _addQueue;
        private readonly Queue<T> _removeQueue;
        private readonly IComparer<T> _tickableComparer;
        
        public Type ServicedTickType => typeof(T);

        protected IReadOnlyList<T> Tickables => _tickables;

        public TickController()
        {
            _tickables = new List<T>();
            _addQueue = new Queue<T>();
            _removeQueue = new Queue<T>();
            _tickableComparer = new TickableComparer();
        }
        
        public bool Add(IBaseTickable value)
        {
            if (value is T genericTickable && !_addQueue.Contains(genericTickable))
            {
                _addQueue.Enqueue(genericTickable);
                return true;
            }

            return false;
        }

        public void Processing()
        {
            AddProcessing();
            RemoveProcessing();
            TickProcessing();
        }

        public void Remove(IBaseTickable value)
        {
            if (value is not T genericTickable)
            {
                return;
            }

            if (!_removeQueue.Contains(genericTickable))
            {
                _removeQueue.Enqueue(genericTickable);
            }
        }

        protected abstract void TickProcessing();

        protected void CallWithProfile(Action<float> action, float delta)
        {
            #if UNITY_EDITOR
            UnityEngine.Profiling.Profiler.BeginSample($"{GetType().Name}.Tick()");
            #endif
            action.Invoke(delta);
            #if UNITY_EDITOR
            UnityEngine.Profiling.Profiler.EndSample();
            #endif
        }

        private void AddProcessing()
        {
            while (_addQueue.TryDequeue(out T tickable))
            {
                if (_tickables.Contains(tickable))
                {
                    continue;
                }
                
                _tickables.Add(tickable);
            }
            
            _tickables.Sort(_tickableComparer);
        }

        private void RemoveProcessing()
        {
            while (_removeQueue.TryDequeue(out T tickable))
            {
                if (!_tickables.Contains(tickable))
                {
                    continue;
                }

                _tickables.Remove(tickable);
            }
            
            _tickables.Sort(_tickableComparer);
        }
    }
}