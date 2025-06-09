using Assets.CodeBase.Logic.DynamicDataLogic;
using Assets.CodeBase.StaticData;
using Assets.SimpleLocalization.Scripts;
using CodeBase.UI.Elements.Bars;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

namespace CodeBase.UI.Elements.Hud
{
    public class BossDataPresenter : MonoBehaviour
    {
        [SerializeField] [Required()] private HpBar _hpBar;

        [SerializeField] [Required()] TextMeshProUGUI _bossNameText;

        public void Construct(Health bossHealth, BossStaticData staticData)
        {
            _bossNameText.text = LocalizationManager.Localize(staticData.BossNameKey);
            _hpBar.Construct(bossHealth);
        }

        public void Hide() => SetVisibility(false);

        public void Show() => SetVisibility(true);

        private void SetVisibility(bool value)
        {
            _bossNameText.gameObject.SetActive(value);
            _hpBar.gameObject.SetActive(value);
        }
    }
}