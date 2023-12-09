using System;
using System.Collections.Generic;
using MbsCore.TickSystem.Infrastructure;
using UnityEngine;

namespace MbsCore.TickSystem.Runtime
{
    internal sealed class TickMonoKernel : MonoBehaviour, IDisposable
    {
        public enum TickType
        {
            Default = 0,
            Fix = 1,
            Late = 2,
        }

        private readonly Dictionary<TickType, ITickController> _controllers = new();

        private bool IsInitialized { get; set; } = false;
        
        public void Initialize(Dictionary<TickType, ITickController> controllers)
        {
            if (IsInitialized)
            {
                return;
            }
            
            _controllers.Clear();
            foreach (var controller in controllers)
            {
                _controllers.Add(controller.Key, controller.Value);
            }

            IsInitialized = false;
        }
        
        public void Dispose()
        {
            if (!IsInitialized)
            {
                return;
            }

            IsInitialized = false;
            _controllers.Clear();
        }

        private void FixedUpdate()
        {
            if (!_controllers.TryGetValue(TickType.Fix, out ITickController controller))
            {
                return;
            }
            
            controller.Processing();
        }

        private void Update()
        {
            if (!_controllers.TryGetValue(TickType.Default, out ITickController controller))
            {
                return;
            }
            
            controller.Processing();
        }

        private void LateUpdate()
        {
            if (!_controllers.TryGetValue(TickType.Late, out ITickController controller))
            {
                return;
            }
            
            controller.Processing();
        }

        private void OnDestroy()
        {
            Dispose();
        }
    }
}