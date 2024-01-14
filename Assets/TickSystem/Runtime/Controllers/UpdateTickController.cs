using UnityEngine;
using UnityEngine.PlayerLoop;

namespace MbsCore.TickSystem
{
    internal sealed class UpdateTickController : TickController<Update>
    {
        protected override float DeltaTime => Time.deltaTime;
    }
}