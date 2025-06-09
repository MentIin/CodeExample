using UnityEngine;

namespace CodeBase.UI.Elements.Bars
{
    class MultiBar : Bar
    {
        [SerializeField] private Bar[] _bars;
        public override void Initialize()
        {
            
        }

        public override void SetValue(float current, float max)
        {
            foreach (var bar in _bars)
            {
                bar.SetValue(current, max);
            }
        }
    }
}