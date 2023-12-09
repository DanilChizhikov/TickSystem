using MbsCore.TickSystem.Infrastructure;
using UnityEngine;

namespace MbsCore.TickSystem.Runtime.Controllers
{
    internal sealed class DefaultTickController : TickController<ITickable>
    {
        protected override void TickProcessing()
        {
            for (int i = Tickables.Count - 1; i >= 0; i--)
            {
                CallWithProfile(Tickables[i].Tick, Time.deltaTime);
            }
        }
    }
}