using System.Collections.Generic;
using Assets.CodeBase.Infrastructure.Factory;
using Assets.CodeBase.Infrastructure.Services.PersistentProgress;
using Assets.CodeBase.Infrastructure.Services.Random;
using Assets.CodeBase.Infrastructure.Services.StaticData;
using Assets.CodeBase.Logic.DynamicDataLogic;
using Assets.CodeBase.StaticData;
using Assets.CodeBase.StaticData.Level;
using UnityEngine;

namespace CodeBase.Logic.Collectables
{
    public class Drop : MonoBehaviour
    {
        private Death _death;
        private IPersistentProgressService _progressService;
        private IGameFactory _gameFactory;
        private IStaticDataService _staticData;
        private IRandomService _randomService;
        private LevelStaticData _currentLevel;

        public void Construct(IPersistentProgressService progressService, IGameFactory gameFactory,
            IStaticDataService staticDataService, Death death, IRandomService randomService,
            LevelStaticData currentLevel)
        {
            _currentLevel = currentLevel;
            _randomService = randomService;
            _staticData = staticDataService;
            _death = death;
            _progressService = progressService;
            _gameFactory = gameFactory;
            
            _death.Happen += DeathOnHappen;
        }

        private void DeathOnHappen()
        {
            _gameFactory.SpawnModuleCollectable(_randomService.Choose(
                GetAvaliableModulesToUnlock(_currentLevel).ToArray()),
                _death.transform.position
                );
        }
        
        
        private List<MachineModuleStaticData> GetAvaliableModulesToUnlock(LevelStaticData staticData)
        {
            List<MachineModuleStaticData> datas = new List<MachineModuleStaticData>();
            foreach (var levelStaticData in _staticData.GetAllLevels().LevelsStaticData)
            {
                foreach (var data in levelStaticData.ModulesToUnlock)
                {
                    if (_progressService.Progress.GetModuleLevel(data.Id) < GameConstants.MaxModuleLevel)
                    {
                        datas.Add(data);
                    }
                }
                if (levelStaticData == staticData)
                {
                    break;
                }
            }

            if (datas.Count == 0)
            {
                foreach (var levelStaticData in _staticData.GetAllLevels().LevelsStaticData)
                {
                    foreach (var data in levelStaticData.ModulesToUnlock)
                    {
                        datas.Add(data);
                    }
                    if (levelStaticData == staticData)
                    {
                        break;
                    }
                }
            }
            Debug.Log(datas.Count);
            return datas;
        }

    }
}