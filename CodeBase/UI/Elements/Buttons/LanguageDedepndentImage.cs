using Assets.SimpleLocalization.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.UI.Elements.Buttons
{
    public class LanguageDependentImage : MonoBehaviour
    {
        [SerializeField] private Image _image;

        [SerializeField] private Sprite[] _images;

        public void OnEnable()
        {
            LocalizationManager.OnLocalizationChanged += UpdateFlag;
            UpdateFlag();
        }

        private void OnDisable()
        {
            LocalizationManager.OnLocalizationChanged -= UpdateFlag;
        }
        

        private void UpdateFlag()
        {
            
            if (LocalizationManager.Language == "Russian")
            {
                _image.sprite = _images[1];
            }else
            {
                _image.sprite = _images[0];
            }
        }
    }
}