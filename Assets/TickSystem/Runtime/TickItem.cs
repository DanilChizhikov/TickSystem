using System;

namespace MbsCore.TickSystem
{
    internal sealed class TickItem
    {
        public int Order { get; }
        public object Owner { get; }
        public Action<float> Action { get; }

        public TickItem(int order, object owner, Action<float> action)
        {
            Order = order;
            Owner = owner;
            Action = action;
        }
    }
}