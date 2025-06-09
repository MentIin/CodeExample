using System;
using System.Collections.Generic;
using CodeBase.Infrastructure.Services.Analytics;
using CodeBase.UI.Services;
using UnityEngine;

namespace CodeBase.UI
{
    public class WindowCloseAnalytic : MonoBehaviour
    {
        private IAnalyticsService _analyticsService;
        private WindowType _type;

        public void Construct(IAnalyticsService analyticsService, WindowType type)
        {
            _type = type;
            _analyticsService = analyticsService;
        }

        private void OnDestroy()
        {
            _analyticsService.InvokeEvent("WindowClosed", new Dictionary<string, string>()
            {
                {"WindowType", _type.ToString()}
            });
        }
    }
}