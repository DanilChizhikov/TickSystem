namespace MbsCore.TickSystem.Infrastructure
{
    public interface IFixTickable : IBaseTickable
    {
        void FixTick(float deltaTime);
    }
}