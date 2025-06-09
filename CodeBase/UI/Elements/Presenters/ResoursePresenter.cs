using Assets.CodeBase.Logic;
using Assets.CodeBase.StaticData;
using Assets.SimpleLocalization.Scripts;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.UI.Elements.Presenters
{
    public class ResourcePresenter : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private TextMeshProUGUI _countText;
        [SerializeField] private bool _showName=false;
        [ShowIf("_showName")] [SerializeField]private TextMeshProUGUI _nameText;
        
        private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        public void SetData(MaterialStaticData staticData)
        {
            if (!_rectTransform) _rectTransform = GetComponent<RectTransform>();
            _image.SetSpriteAndPivot(_rectTransform, staticData.Icon);

            if (_showName)
            {
                _nameText.text = LocalizationManager.Localize(staticData.NameKey);
            }
        }

        public void SetAmount(int amount)
        {
            _countText.text = "x" + amount.ToString();
        }
    }
}