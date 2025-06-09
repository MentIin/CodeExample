using Assets.CodeBase.Infrastructure.Data;
using Assets.CodeBase.Infrastructure.Data.Stats;
using Assets.CodeBase.Logic;
using Assets.CodeBase.StaticData;
using Assets.SimpleLocalization.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.UI.Elements.Presenters
{
    public class StatPresenter : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _amountText;
        private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        public void Present(Sprite icon, Stat stat, StatsDisplayType type)
        {
            _icon.SetSpriteAndPivot(_rectTransform, icon);

            string amountText = stat.Value.ToString();
            if (type == StatsDisplayType.Seconds)
            {
                amountText = ((float)stat.Value.FromMillisecondsToSeconds()) + " " +
                             LocalizationManager.Localize("Stats.Seconds");
            }
            _amountText.text = amountText;
        }

        public void Present(int present)
        {
            _amountText.text = present.ToString();
        }
    }
}