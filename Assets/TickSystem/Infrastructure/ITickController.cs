using System;

namespace MbsCore.TickSystem.Infrastructure
{
    public interface ITickController
    {
        Type ServicedTickType { get; }

        bool Add(IBaseTickable value);
        void Processing();
        void Remove(IBaseTickable value);
    }
}