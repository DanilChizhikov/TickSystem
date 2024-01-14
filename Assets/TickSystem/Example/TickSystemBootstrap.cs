using UnityEngine;

namespace MbsCore.TickSystem
{
    public sealed class TickSystemBootstrap : MonoBehaviour
    {
        private static ITickService _tickService;

        public static ITickService TickService => _tickService;

        private void Awake()
        {
            if (TickService != null)
            {
                Destroy(gameObject);
                return;
            }

            _tickService = new TickService();
            DontDestroyOnLoad(gameObject);
        }
    }
}