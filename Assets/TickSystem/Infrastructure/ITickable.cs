namespace MbsCore.TickSystem
{
    public interface ITickable : IBaseTickable
    {
        void Tick(float deltaTime);
    }
}