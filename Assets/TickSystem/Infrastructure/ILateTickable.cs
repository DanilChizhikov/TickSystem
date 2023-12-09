namespace MbsCore.TickSystem.Infrastructure
{
    public interface ILateTickable : IBaseTickable
    {
        void LateTick(float deltaTime);
    }
}