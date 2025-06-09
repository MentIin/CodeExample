using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.CodeBase.Logic
{
    public static class LogicExtensions
    {
        public static void SetSpriteAndPivot(this Image image, RectTransform rectTransform, Sprite sprite)
        {
            image.sprite = sprite;

            rectTransform.pivot = sprite.pivot / sprite.rect.size;
        }

        public static bool CheckIfLayerInMask(this LayerMask layerMask, int layer)
        {
            return layerMask == (layerMask | (1 << layer));
        }


        public static void NormalizeText(this TextMeshProUGUI textMeshProUGUI)
        {
            textMeshProUGUI.text = textMeshProUGUI.text.Substring(0, 1).ToUpper() +
                textMeshProUGUI.text.Substring(1, textMeshProUGUI.text.Length - 1);
        }
    }
}