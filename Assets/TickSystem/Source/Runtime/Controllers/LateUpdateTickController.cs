using UnityEngine;
using UnityEngine.PlayerLoop;

namespace DTech.TickSystem
{
    internal sealed class LateUpdateTickController : TickController<PreLateUpdate>
    {
        protected override float DeltaTime => Time.deltaTime;
    }
}