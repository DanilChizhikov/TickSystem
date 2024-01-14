using System;

namespace MbsCore.TickSystem
{
    internal static class TickUtility
    {
        public static void InvokeWithProfile(this object sender, Action<float> tick, float deltaTime)
        {
            #if UNITY_EDITOR
            UnityEngine.Profiling.Profiler.BeginSample($"{sender.GetType().Name}.{tick.GetType().Name}");
            #endif
            tick.Invoke(deltaTime);
            #if UNITY_EDITOR
            UnityEngine.Profiling.Profiler.EndSample();
            #endif
        }
    }
}