namespace DTech.TickSystem
{
    public interface ILateTickable
    {
        void LateTick(float deltaTime);
    }
}