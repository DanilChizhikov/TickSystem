namespace MbsCore.TickSystem
{
    public interface IFixTickable : IBaseTickable
    {
        void FixTick(float deltaTime);
    }
}