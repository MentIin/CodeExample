using System.Collections.Generic;
using Assets.CodeBase.Infrastructure.Data.Loot;
using Assets.CodeBase.Infrastructure.Services.PersistentProgress;
using Assets.CodeBase.Infrastructure.Services.StaticData;
using CodeBase.DebugLogic;
using UnityEngine;

namespace CodeBase.UI.Elements.Presenters
{
    public class PlayerResourcesPresenter : MonoBehaviour
    {
        [SerializeField] private ResourcePresenter _resourcePresenterPrefab;
        private IPersistentProgressService _progressService;
        private IStaticDataService _staticDataService;
        private Dictionary<CraftingResourceType, ResourcePresenter> _presenters;

        public void Construct(IPersistentProgressService progressService, IStaticDataService staticDataService)
        {
            _presenters = new Dictionary<CraftingResourceType, ResourcePresenter>();
            _progressService = progressService;
            _progressService.Progress.ResourcesChanged += UpdateAmount;
            _staticDataService = staticDataService;
            Initialize();
        }

        private void OnDestroy()
        {
            _progressService.Progress.ResourcesChanged -= UpdateAmount;
        }

        private void Initialize()
        {
            
            foreach (var pair in _progressService.Progress.Resources)
            {
                ResourcePresenter presenter = Instantiate(_resourcePresenterPrefab, transform);
                _presenters[pair.Key] = presenter;
                presenter.SetData(_staticDataService.ForMaterial(pair.Key));
                presenter.SetAmount(pair.Value);
            }
            UpdateAmount();
        }

        private void UpdateAmount()
        {
            
            foreach (var pair in _presenters)
            {
                int progressResource = _progressService.Progress.Resources[pair.Key];
                if (progressResource == 0)
                {
                    pair.Value.gameObject.SetActive(false);
                }
                else
                {
                    pair.Value.SetAmount(progressResource);
                    pair.Value.gameObject.SetActive(true);
                }
            }
        }

        private void Update()
        {
            
            if (Input.GetKeyDown(KeyCode.C))
            {
                if (DebugSinglton.Instance.InfinityResources)
                {
                    LootData loot = new LootData{Amount = 100, CraftingResourceType = CraftingResourceType.Copper,
                        Type = LootType.CraftingResource};
                    _progressService.Progress.CollectResource(loot);
                
                    loot = new LootData{Amount = 100, CraftingResourceType = CraftingResourceType.Iron,
                        Type = LootType.CraftingResource};
                    _progressService.Progress.CollectResource(loot);
                
                    loot = new LootData{Amount = 100, CraftingResourceType = CraftingResourceType.Gold,
                        Type = LootType.CraftingResource};
                    _progressService.Progress.CollectResource(loot);
                
                    
                    loot = new LootData{Amount = 100, CraftingResourceType = CraftingResourceType.EssenceOfLife,
                        Type = LootType.CraftingResource};
                    _progressService.Progress.CollectResource(loot);
                
                    loot = new LootData{Amount = 100, CraftingResourceType = CraftingResourceType.EssenceOfDestruction,
                        Type = LootType.CraftingResource};
                    _progressService.Progress.CollectResource(loot);
                
                    loot = new LootData{Amount = 100, CraftingResourceType = CraftingResourceType.EssenceOfPoison,
                        Type = LootType.CraftingResource};
                        
                    _progressService.Progress.CollectResource(loot);
                }

            }
        }
    }
}