using System;

namespace MbsCore.TickSystem
{
    internal sealed class TickItem
    {
        public int Order { get; }
        public Action<float> Action { get; }

        public TickItem(int order, Action<float> action)
        {
            Order = order;
            Action = action;
        }
    }
}