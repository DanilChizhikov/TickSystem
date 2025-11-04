using UnityEngine;
using UnityEngine.PlayerLoop;

namespace DTech.TickSystem
{
    internal sealed class UpdateTickController : TickController<Update>
    {
        protected override float DeltaTime => Time.deltaTime;
    }
}