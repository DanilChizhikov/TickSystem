using UnityEngine;

namespace MbsCore.TickSystem
{
    public class TickEntityFactory : MonoBehaviour
    {
        private TickableEntity _entity;

        private void Awake()
        {
            _entity = new TickableEntity(TickSystemBootstrap.TickService);
        }

        private void OnDestroy()
        {
            _entity?.Dispose();
        }
    }
}