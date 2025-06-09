using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.UI.Elements.Bars
{
    public class DefaultBar : Bar
    {
        public SlicedFilledImage ImageCurrent;

        public bool _havePointer = false;
        public bool _changeColor = false;
        [ShowIf("_havePointer")]public RectTransform _pointer;
        [ShowIf("_havePointer")]public RectTransform _pointerZone;
        [ShowIf("_changeColor")]public Gradient _gradient;

        private TweenerCore<float, float, FloatOptions> tweenerCore;
        public override void Initialize()
        {
            
        }

        public override void SetValue(float current, float max)
        {
            //ImageCurrent.fillAmount = current / max;
            float percent = current / max;
            tweenerCore = DOTween.To(() => ImageCurrent.fillAmount, x => ImageCurrent.fillAmount = x,
                percent, 1f).SetEase(Ease.Flash).SetId(333);

            if (_havePointer)
            {
                Vector2 pos = _pointerZone.position;
                pos = (Vector2)_pointerZone.position +
                      (Vector2)(Vector2.right * _pointerZone.rect.size.x * (percent - 0.5f));
                _pointer.position = new Vector3(pos.x, _pointer.position.y, _pointer.position.z);
            }

            if (_changeColor)
            {
                ImageCurrent.color = _gradient.Evaluate(percent);
            }
        }

        private void OnDestroy()
        {
            DOTween.Kill(333);
        }
    }
}