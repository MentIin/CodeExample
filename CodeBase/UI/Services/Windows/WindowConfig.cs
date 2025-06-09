using System;
using UnityEngine.AddressableAssets;

namespace CodeBase.UI.Services
{
    [Serializable]
    public class WindowConfig
    {
        public WindowType WindowType;
        public AssetReference WindowReference;
    }
}