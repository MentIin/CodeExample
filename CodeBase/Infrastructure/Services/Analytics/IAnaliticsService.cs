using System;
using System.Collections.Generic;
using Assets.CodeBase.Infrastructure.Services;
using UnityEngine;

namespace CodeBase.Infrastructure.Services.Analytics
{
    public interface IAnalyticsService : IService
    {
        void Initialize();
        void InvokeEvent(string name);
        void InvokeEvent(string name, Dictionary<string, string> parameters);
        void SetProperty(string currentmachine, string toString);
        event Action Initialized;
        bool HaveInitialized { get;  }
    }

    public class MockAnalyticsService : IAnalyticsService
    {
        public void Initialize()
        {
            Debug.Log("MockAnalyticsService Initialized");
        }

        public void InvokeEvent(string name)
        {
            
        }

        public void InvokeEvent(string name, Dictionary<string, string> parameters)
        {
            
        }

        public void SetProperty(string currentmachine, string toString)
        {
            
        }

        public event Action Initialized;
        public bool HaveInitialized { get; }
    }
}