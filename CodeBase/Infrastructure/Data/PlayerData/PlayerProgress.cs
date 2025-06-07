using System;
using System.Collections.Generic;
using System.Linq;
using Assets.CodeBase.Infrastructure.Data.Loot;
using Assets.CodeBase.Infrastructure.Services.StaticData;
using Assets.CodeBase.StaticData;
using CodeBase.Infrastructure.Data;
using CodeBase.Infrastructure.Data.PlayerData;
using CodeBase.Logic;
using CodeBase.UI.Elements.Presenters.Machine;
using CodeBase.UI.Services.GameStopOver;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Serialization;

namespace Assets.CodeBase.Infrastructure.Data.PlayerData
{
    [Serializable]
    public class PlayerProgress
    {
        public SettingsData Settings;
        public TutorialData TutorialData;
        public StatisticData StatisticData;

        public bool HaveSave = false;
        
        public int LevelStage;
        public SerializableDictionary<CraftingResourceType, int> Resources;
        
        [SerializeField]private List<OwningModule> _owningModules;
        [SerializeField]private int _darkAmethyst;
        
        [SerializeField]private int _startingMachineId;
        [SerializeField]public SerializableDictionary<int, MachineModel> ShopMachineModels;
        public List<string> AvaliableModules = new List<string>();
        
        
        public List<OwningModule> OwningModules { get => _owningModules; }
        public int DarkAmethyst { get => _darkAmethyst; }

        public MachineModel PlayerMachine
        {
            get
            {
                // to make sure it is bounded
                _currentMachineIndex = Math.Clamp(_currentMachineIndex, 0, _playerMachines.Count-1);
                return _playerMachines[_currentMachineIndex];
            }
        }

        [SerializeField]private List<MachineModel> _playerMachines = new List<MachineModel>();
        [SerializeField]private int _currentMachineIndex;

        public int AmethistsMined;


        public int StartingMachineId
        {
            get => _startingMachineId;
            set
            {
                _startingMachineId = value;
                StartingMachineIdChanged?.Invoke();
            }
        }

        public string Language="English";

        public DifficultyLevel DifficultyLevel = DifficultyLevel.Medium;

        public DifficultyLevel DifficultyChoosen = DifficultyLevel.Medium;

        public int CurrentMachineIndex
        {
            get => _currentMachineIndex;
        }

        public List<MachineModel> PlayerMachines { get => _playerMachines; }

        public event Action StartingMachineIdChanged;

        public event Action OwningModulesChanged;
        public event Action OwningMachinesChanged;
        public event Action ResourcesChanged;
        public event Action<MachineModuleStaticData> ModuleCollected;
        public event Action DarkAmethystAmountChanged;
        public event Action MachineChanged;
        public event Action<UpgradeType, int, string> UpgradeBought;


        public PlayerProgress(int levelStage)
        {
            Settings = new SettingsData();
            TutorialData = new TutorialData();
            StatisticData = new StatisticData();
            
            _owningModules = new List<OwningModule>();
            LevelStage = levelStage;
            Resources = new SerializableDictionary<CraftingResourceType, int>();
            ShopMachineModels = new SerializableDictionary<int, MachineModel>();


            ClearResources();
        }

        public void ClearResources()
        {
            Resources = new SerializableDictionary<CraftingResourceType, int>();
            foreach (var VARIABLE in Enum.GetValues(typeof(CraftingResourceType)))
            {
                Resources[(CraftingResourceType)VARIABLE] = 0;
            }
        }

        public bool EnoughMaterials(CraftMaterialInfo[] dataCraft)
        {
            foreach (var info in dataCraft)
            {
                if (info.Amount > Resources[info.Material])
                {
                    return false;
                }
            }
            return true;
        }

        public void CollectResource(LootData lootData)
        {
            if (lootData.Type == LootType.CraftingResource)
            {
                if (Resources.ContainsKey(lootData.CraftingResourceType)) Resources[lootData.CraftingResourceType] += lootData.Amount;
                else Resources[lootData.CraftingResourceType] = lootData.Amount;
                ResourcesChanged?.Invoke();
            }else if (lootData.Type == LootType.DarkAmethyst)
            {
                _darkAmethyst += lootData.Amount;
                AmethistsMined++;
                DarkAmethystAmountChanged?.Invoke();
            }
            
        }

        public void CraftModule(MachineModuleStaticData data, int level)
        {
            _owningModules.Add(new OwningModule(data.Id, level));
            

            if (level == 1)
            {
                OwningModulesChanged?.Invoke();
                foreach (var info in data.Craft)
                {
                    Resources[info.Material] -= info.Amount;
                }
                ResourcesChanged?.Invoke();
            }
            else
            {
                DeleteModule(data.Id, level-1);
                DeleteModule(data.Id, level-1);
                OwningModulesChanged?.Invoke();
            }
            
        }

        private void DeleteModule(string dataId, int level)
        {
            OwningModule toDel = null;
            foreach (var owningModule in _owningModules)
            {
                if (owningModule.Id == dataId && owningModule.Level == level)
                {
                    toDel = owningModule;
                    break;
                }
            }
            
            _owningModules.Remove(toDel);
        }



        public void RemoveOwningModule(string id, int level)
        {
            OwningModule toRem = null;
            foreach (var owning in _owningModules)
            {
                if (owning.Id == id && owning.Level == level)
                {
                    toRem = owning;
                }
            }
            _owningModules.Remove(toRem);
            OwningModulesChanged?.Invoke();
        }
        public void RemoveMachineModule(int place)
        {
            foreach (var info in PlayerMachine.MachineModules)
            {
                if (info.Place == place)
                {

                    _owningModules.Add(new OwningModule(info.Id, info.Level));

                    
                    OwningModulesChanged?.Invoke();
                    PlayerMachine.RemoveMachineModule(place);
                    
                    return;
                }
            }
        }
        public void InstallOwningModule(string id, int level, int place)
        {
            if (PlayerMachine.HaveFreePlace(place))
            {
                string dataId = id; 
                PlayerMachine.InstallModule(dataId, level, place);
                RemoveOwningModule(id, level);
            }
        }

        public void ObtainMachine(MachineModel machineModel)
        {
            _playerMachines.Add(machineModel);
            _currentMachineIndex = _playerMachines.Count - 1;
        }

        public MachineModel GetShopModel(MachineStaticData staticData)
        {
            if (ShopMachineModels.ContainsKey(staticData.Id)) return ShopMachineModels[staticData.Id];
            
            MachineModel model = new MachineModel();
            model.MachineBasementId = staticData.BasementStaticData.Id;
            model.MachineModules = staticData.Modules.AsModuleSerializableInfo();

            ShopMachineModels[staticData.Id] = model;
            return model;

        }

        public void ClearOwningModules()
        {
            _owningModules = new List<OwningModule>();
        }
        
        /// <param name="level">starting from 1</param>
        public void ObtainModule(MachineModuleStaticData staticData, int level)
        {
            ObtainModule(staticData, level, true);
        }
        /// <param name="level">starting from 1</param>
        public void ObtainModule(MachineModuleStaticData staticData, int level, bool addToInventory)
        {
            if (level - 1 == GetModuleLevel(staticData.Id))
            {
                AvaliableModules.Add(staticData.Id);
            }

            if (addToInventory)
            {
                Debug.Log(level);
                _owningModules.Add(new OwningModule(staticData.Id, level));
            }

            ModuleCollected?.Invoke(staticData);
            OwningModulesChanged?.Invoke();
        }

        public void Upgrade(UpgradeType upgradeType, MachineBasementStaticData forStartingMachine)
        {
            int price = forStartingMachine.GetUpgradePrice(upgradeType,
                PlayerMachine.GetUpgradeLevel(upgradeType));
                        ;

            _darkAmethyst -= price;
            DarkAmethystAmountChanged?.Invoke();
            PlayerMachine.Upgrades.Add(upgradeType);

            int upgradesCount = PlayerMachine.GetUpgradeLevel(upgradeType);
            UpgradeBought?.Invoke(upgradeType, upgradesCount, PlayerMachine.MachineBasementId.ToString());
            
        }

        public bool HaveModule(string dataId, int lvl)
        {
            foreach (var owningModule in _owningModules)
            {
                if (owningModule.Id == dataId && owningModule.Level == lvl)
                {
                    return true;
                }
            }

            return false;
        }

        public int CountModules(string dataId, int moduleLevel)
        {
            int res = 0;
            foreach (var VARIABLE in _owningModules)
            {
                if (VARIABLE.Id == dataId && VARIABLE.Level == moduleLevel)
                {
                    res++;
                }
            }

            return res;
        }

        public void SetAmethyst(int i)
        {
            _darkAmethyst = i;
        }

        public int GetModuleLevel(string dataId)
        {
            int res = 0;
            foreach (var owningModule in AvaliableModules)
            {
                if (owningModule == dataId)
                {
                    res++;
                }
            }

            if (res > GameConstants.MaxModuleLevel)
            {
                res = GameConstants.MaxModuleLevel;
            }
            return res;
        }

        public void ChangeMachine(ChangeType changeType)
        {
            if (changeType == ChangeType.Right)
            {
                SetPlayerMachineModelIndex(_currentMachineIndex +1);
            }
            else
            {
                SetPlayerMachineModelIndex(_currentMachineIndex -1);
            }
        }

        public void SetPlayerMachineModelIndex(int id)
        {
            if (id < 0)
            {
                id += _playerMachines.Count;
            }

            if (id >= _playerMachines.Count)
            {
                id -= _playerMachines.Count;
            }
            _currentMachineIndex = id;
            
            MachineChanged?.Invoke();
        }

        public void RemoveMachineByBasementId(int id)
        {
            foreach (var machineModel in _playerMachines)
            {
                if (id == machineModel.MachineBasementId)
                {
                    _playerMachines.Remove(machineModel);
                    if (CheckIfHaveMachineByBasementId(id))
                    {
                        RemoveMachineByBasementId(id);
                    }

                    if (_currentMachineIndex >= _playerMachines.Count)
                    {
                        SetPlayerMachineModelIndex(_playerMachines.Count-1);
                    }
                    return;
                }
            }

            
        }

        public bool CheckIfHaveMachineByBasementId(int id)
        {
            foreach (var machineModel in _playerMachines)
            {
                if (id == machineModel.MachineBasementId)
                {
                    return true;
                }
            }

            return false;
        }
    }
}