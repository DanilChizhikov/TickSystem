using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.LowLevel;

namespace MbsCore.TickSystem
{
    internal abstract class TickController<TUpdate> : ITickController
    {
        private static readonly IComparer<TickItem> s_tickComparer = new TickComparer();
        
        private readonly List<TickItem> _ticks;
        private readonly Queue<TickItem> _addQueue;
        private readonly Queue<TickItem> _removeQueue;
        private readonly StringBuilder _profileNameBuilder;
        
        protected abstract float DeltaTime { get; }
        
        private int TickCount { get; set; }
        
        public TickController()
        {
            _ticks = new List<TickItem>();
            _addQueue = new Queue<TickItem>();
            _removeQueue = new Queue<TickItem>();
            _profileNameBuilder = new StringBuilder();
            TickCount = 0;
            PlayerLoopExtensions.ModifyPlayerLoop((ref PlayerLoopSystem system) =>
            {
                system.GetSystem<TUpdate>().AddSystem(GetType(), TickProcessing);
            });
        }
        
        public bool TryAdd(object owner, Action<float> tick, int order)
        {
            if (Contains(tick))
            {
                return false;
            }
            
            _addQueue.Enqueue(new TickItem(order, owner, tick));
            return true;
        }

        public bool TryRemove(Action<float> tick)
        {
            if (!Contains(tick))
            {
                return false;
            }

            int tickIndex = IndexOf(tick);
            _removeQueue.Enqueue(_ticks[tickIndex]);
            return true;
        }
        
        public void Dispose()
        {
            PlayerLoopExtensions.ModifyPlayerLoop((ref PlayerLoopSystem system) =>
            {
                system.GetSystem<TUpdate>().RemoveSystem(GetType(), false);
            });
            
            _addQueue.Clear();
            _removeQueue.Clear();
            _ticks.Clear();
            TickCount = 0;
        }

        private bool Contains(Action<float> tick)
        {
            int tickIndex = IndexOf(tick);
            return tickIndex >= 0;
        }

        private int IndexOf(Action<float> tick)
        {
            for (int i = _ticks.Count - 1; i >= 0; i--)
            {
                TickItem tickItem = _ticks[i];
                if (tickItem.Action.Equals(tick))
                {
                    return i;
                }
            }

            return -1;
        }

        private void AddProcessing(ref bool isDirty)
        {
            while (_addQueue.TryDequeue(out TickItem item))
            {
                if (!_ticks.Contains(item))
                {
                    _ticks.Add(item);
                    isDirty = true;
                }
            }
        }
        
        private void RemoveProcessing(ref bool isDirty)
        {
            while (_removeQueue.TryDequeue(out TickItem item))
            {
                int itemIndex = _ticks.IndexOf(item);
                if (itemIndex >= 0)
                {
                    _ticks.RemoveAt(itemIndex);
                    isDirty = true;
                }
            }
        }
        
        private void PreTickProcessing()
        {
            bool isDirty = false;
            AddProcessing(ref isDirty);
            RemoveProcessing(ref isDirty);
            if (isDirty)
            {
                _ticks.Sort(s_tickComparer);
                TickCount = _ticks.Count;
            }
        }

        private string GetProfileName(TickItem item)
        {
            _profileNameBuilder.Clear();
            _profileNameBuilder.Append(item.Owner.GetType().Name);
            _profileNameBuilder.Append('.');
            _profileNameBuilder.Append(item.Action.Method.Name);
            _profileNameBuilder.Append("()");
            return _profileNameBuilder.ToString();
        }
        
        private void TickProcessing()
        {
            PreTickProcessing();
            for (int i = 0; i < TickCount; i++)
            {
                #if UNITY_EDITOR
                UnityEngine.Profiling.Profiler.BeginSample(GetProfileName(_ticks[i]));
                #endif
                _ticks[i].Action.Invoke(DeltaTime);
                #if UNITY_EDITOR
                UnityEngine.Profiling.Profiler.EndSample();
                #endif
            }
        }
    }
}