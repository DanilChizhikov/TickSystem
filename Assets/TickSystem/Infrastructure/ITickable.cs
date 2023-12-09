namespace MbsCore.TickSystem.Infrastructure
{
    public interface ITickable : IBaseTickable
    {
        void Tick(float deltaTime);
    }
}