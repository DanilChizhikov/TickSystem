using UnityEngine;
using UnityEngine.PlayerLoop;

namespace MbsCore.TickSystem
{
    internal sealed class FixedUpdateTickController : TickController<FixedUpdate>
    {
        protected override float DeltaTime => Time.fixedDeltaTime;
    }
}