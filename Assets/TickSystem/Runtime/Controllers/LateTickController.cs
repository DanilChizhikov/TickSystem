using MbsCore.TickSystem.Infrastructure;
using UnityEngine;

namespace MbsCore.TickSystem.Runtime.Controllers
{
    internal sealed class LateTickController : TickController<ILateTickable>
    {
        protected override void TickProcessing()
        {
            for (int i = Tickables.Count - 1; i >= 0; i--)
            {
                CallWithProfile(Tickables[i].LateTick, Time.deltaTime);
            }
        }
    }
}