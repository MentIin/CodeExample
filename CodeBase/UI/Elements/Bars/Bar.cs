using UnityEngine;

namespace CodeBase.UI.Elements.Bars
{
    public abstract class Bar : MonoBehaviour
    {
        public abstract void Initialize();
        public abstract void SetValue(float current, float max);
    }
}