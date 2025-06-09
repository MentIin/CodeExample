using Assets.CodeBase.Infrastructure.Data.Loot;
using Assets.CodeBase.Infrastructure.Services.AdsService;
using Assets.CodeBase.Infrastructure.Services.PersistentProgress;
using Assets.CodeBase.Infrastructure.Services.SaveLoad;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CodeBase.UI.Elements.Buttons
{
    public class AdButton : MonoBehaviour, IPointerClickHandler
    {
        private IAdsService _adsService;
        private IPersistentProgressService _persistentProgressService;
        private ISaveLoadService _saveLoadService;

        public void Construct(IAdsService adsService, IPersistentProgressService persistentProgressService,
            ISaveLoadService saveLoadService)
        {
            _saveLoadService = saveLoadService;
            _adsService = adsService;
            _persistentProgressService = persistentProgressService;
            _adsService.RewardedAdEvent += Reward;
        }

        private void OnDestroy()
        {
            _adsService.RewardedAdEvent -= Reward;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _adsService.ShowRewarded(20);
        }

        private void Reward(int id)
        {
            if (id == 20)
            {
                LootData loot = new LootData();
                loot.Type = LootType.DarkAmethyst;
                loot.Amount = 10;
                _persistentProgressService.Progress.CollectResource(loot);
                _saveLoadService.SaveProgress();
            }
        }
    }
}