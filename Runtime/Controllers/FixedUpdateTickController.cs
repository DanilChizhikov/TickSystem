using UnityEngine;
using UnityEngine.PlayerLoop;

namespace DTech.TickSystem
{
    internal sealed class FixedUpdateTickController : TickController<FixedUpdate>
    {
        protected override float DeltaTime => Time.fixedDeltaTime;
    }
}