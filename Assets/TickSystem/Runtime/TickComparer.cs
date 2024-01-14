using System.Collections.Generic;

namespace MbsCore.TickSystem
{
    internal sealed class TickComparer : IComparer<TickItem>
    {
        public int Compare(TickItem x, TickItem y)
        {
            if (ReferenceEquals(x, y))
            {
                return 0;
            }

            if (ReferenceEquals(null, y))
            {
                return 1;
            }

            if (ReferenceEquals(null, x))
            {
                return -1;
            }

            return x.Order.CompareTo(y.Order);
        }
    }
}