using UnityEngine;
using UnityEngine.PlayerLoop;

namespace MbsCore.TickSystem
{
    internal sealed class LateUpdateTickController : TickController<PreLateUpdate>
    {
        protected override float DeltaTime => Time.deltaTime;
    }
}