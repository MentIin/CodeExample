using System;
using CodeBase.UI.Services.UIFactory;

namespace CodeBase.UI.Services
{
    public class WindowService : IWindowService
    {
        private readonly IUIFactory _uiFactory;

        public WindowService(IUIFactory uiFactory)
        {
            _uiFactory = uiFactory;
        }

        public void Open(WindowType type)
        {
            switch (type)
            {
                case WindowType.Unknown:
                    break;
                case WindowType.CraftWindow:
                    _uiFactory.CreateWindow(type);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}