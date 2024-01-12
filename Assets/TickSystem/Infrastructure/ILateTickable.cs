namespace MbsCore.TickSystem
{
    public interface ILateTickable : IBaseTickable
    {
        void LateTick(float deltaTime);
    }
}