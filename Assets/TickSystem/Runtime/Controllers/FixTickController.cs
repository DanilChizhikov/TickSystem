using UnityEngine;

namespace MbsCore.TickSystem
{
    internal sealed class FixTickController : TickController<IFixTickable>
    {
        protected override void TickProcessing()
        {
            for (int i = Tickables.Count - 1; i >= 0; i--)
            {
                CallWithProfile(Tickables[i].FixTick, Time.fixedDeltaTime);
            }
        }
    }
}