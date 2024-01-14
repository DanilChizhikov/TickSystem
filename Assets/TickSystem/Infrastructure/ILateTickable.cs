namespace MbsCore.TickSystem
{
    public interface ILateTickable
    {
        void LateTick(float deltaTime);
    }
}