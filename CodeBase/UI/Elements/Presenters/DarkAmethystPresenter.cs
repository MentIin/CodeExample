using Assets.CodeBase.Infrastructure.Services.PersistentProgress;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.UI.Elements.Presenters
{
    public class DarkAmethystPresenter : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Image _icon;
        [SerializeField] private bool _showOnlyWhenValueChanged=false;
        private IPersistentProgressService _progressService;

        private float _current = 0f;

        public void Construct(IPersistentProgressService progressService)
        {
            _progressService = progressService;

            UpdateText();
            _progressService.Progress.DarkAmethystAmountChanged += UpdateText;
        }

        private void OnDestroy()
        {
            _progressService.Progress.DarkAmethystAmountChanged -= UpdateText;
        }

        private void Update()
        {
            if (!_showOnlyWhenValueChanged) return;

            Color color = new Color(1f, 1f, 1f, _current);
            _icon.color = color;
            _text.color = color;

            
            _current -= Time.deltaTime;
            if (_current < 0) _current = 0f;
        }


        private void UpdateText()
        {
            if (_showOnlyWhenValueChanged) _current = 1f;
            int progressDarkAmethyst = _progressService.Progress.DarkAmethyst;
            _text.text = progressDarkAmethyst.ToString();
        }
    }
}