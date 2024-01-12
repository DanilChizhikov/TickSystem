using System;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace MbsCore.TickSystem
{
    internal sealed class TickKernel : IDisposable
    {
        public ITickController FixTickController { get; }
        public ITickController TickController { get; }
        public ITickController LateTickController { get; }
        
        public TickKernel()
        {
            FixTickController = new FixTickController();
            TickController = new DefaultTickController();
            LateTickController = new LateTickController();
            PlayerLoopExtensions.ModifyPlayerLoop((ref PlayerLoopSystem system) =>
            { 
                system.GetSystem<FixedUpdate>().AddSystem<TickKernel>(FixTick);
                system.GetSystem<Update>().AddSystem<TickKernel>(Tick);
                system.GetSystem<PostLateUpdate>().AddSystem<TickKernel>(LateTick);
            });
        }
        
        public void Dispose()
        {
            PlayerLoopExtensions.ModifyPlayerLoop((ref PlayerLoopSystem system) =>
            { 
                system.GetSystem<FixedUpdate>().RemoveSystem<TickKernel>(false);
                system.GetSystem<Update>().RemoveSystem<TickKernel>(false);
                system.GetSystem<PostLateUpdate>().RemoveSystem<TickKernel>(false);
            });
        }

        private void FixTick()
        {
            FixTickController.Processing();
        }

        private void Tick()
        {
            TickController.Processing();
        }

        private void LateTick()
        {
            LateTickController.Processing();
        }
    }
}