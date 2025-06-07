using System;
using System.Collections;
using System.Collections.Generic;
using Assets.CodeBase.Infrastructure;
using Assets.CodeBase.Infrastructure.Data.PlayerData;
using Assets.CodeBase.Infrastructure.Services.GetUserInfoService;
using Assets.CodeBase.Infrastructure.Services.Pause;
using Assets.CodeBase.Infrastructure.Services.PersistentProgress;
using Assets.CodeBase.Infrastructure.Services.SaveLoad;
using Assets.CodeBase.Infrastructure.Services.StaticData;
using Assets.SimpleLocalization.Scripts;
using CodeBase.Infrastructure.Data;
using CodeBase.Infrastructure.Services.Analitics;
using CodeBase.Infrastructure.Services.Analytics;
using UnityEngine;

namespace CodeBase.Infrastructure.States
{
    public class LoadProgressState : IState
    {
        private GameStateMachine _stateMachine;
        private IPersistentProgressService _progressService;
        private ISaveLoadService _saveLoadService;
        private readonly IStaticDataService _staticDataService;
        private readonly IUserInfoService _userInfoService;
        private readonly IAnalyticsService _analyticsService;
        private readonly PauseService _pauseService;
        private readonly ICoroutineRunner _coroutineRunner;


        private Coroutine _analyticServiceCoroutine;
        private bool _analyticServiceTimeout=false;

        public LoadProgressState(GameStateMachine stateMachine,
            IPersistentProgressService progressService,
            ISaveLoadService saveLoadService, IStaticDataService staticDataService,
             IUserInfoService userInfoService, IAnalyticsService analyticsService,
            PauseService pauseService, ICoroutineRunner coroutineRunner)
        {
            _stateMachine = stateMachine;
            _progressService = progressService;
            _saveLoadService = saveLoadService;
            _staticDataService = staticDataService;
            _userInfoService = userInfoService;
            _analyticsService = analyticsService;
            _pauseService = pauseService;
            _coroutineRunner = coroutineRunner;
        }

        public void Enter()
        {
            _pauseService.Pause();
            _staticDataService.Load();
            if (_saveLoadService.LoadProgress() == null) _progressService.Progress = GetNewProgress();
            else _progressService.Progress = _saveLoadService.LoadProgress();


            FixIncorrectSaves();
            ObtainStartModules();
            
            // require progress
            _analyticsService.Initialize();
            _analyticsService.Initialized += EnterTwo;
            _analyticServiceCoroutine = _coroutineRunner.StartCoroutine(AnalyticCoroutine());
        }

        private IEnumerator AnalyticCoroutine()
        {
            yield return new WaitForSecondsRealtime(3f);
            if (_analyticServiceTimeout)
            {
                
            }
            else
            {
                EnterTwo();
            }
        }

        private void EnterTwo()
        {
            if (_analyticServiceTimeout) return;
            _analyticServiceTimeout = true;
            
            
            List<KeyValuePair<int, MachineModel>> toDelete = new List<KeyValuePair<int, MachineModel>>();
            foreach (var valuePair in _progressService.Progress.ShopMachineModels)
            {
                if (_staticDataService.ForStartingMachine(valuePair.Key) == null)
                {
                    toDelete.Add(valuePair);
                }
                else if (valuePair.Value.MachineModules.Count != _staticDataService.ForStartingMachine(valuePair.Key).Modules.Length)
                {
                    toDelete.Add(valuePair);
                }
            }

            foreach (var keyValuePair in toDelete)
            {
                _progressService.Progress.ShopMachineModels.Remove(keyValuePair.Key);
            }
            
            //_stateMachine.Enter<LoadLevelState, int>(_progressService.Progress.LevelStage);


            InitializeLocalization();

            _pauseService.Resume();
            _stateMachine.Enter<MainMenuState>();
        }

        private void InitializeLocalization()
        {
            if (_userInfoService.GetLanguage() == "ru")
            {
                _progressService.Progress.Language = "Russian";
            }
            else
            {
                _progressService.Progress.Language = "English";
            }
            
            LocalizationManager.Language = _progressService.Progress.Language;
            LocalizationManager.Read();
        }

        private void FixIncorrectSaves()
        {
            foreach (var VARIABLE in _progressService.Progress.AvaliableModules)
            { 
                
            }
        }

        private void ObtainStartModules()
        {
            foreach (var data in _staticDataService.GetAllModules())
            {
                if (data.AvailableOnStart)
                {
                    if (_progressService.Progress.GetModuleLevel(data.Id) == 0)
                    {
                        _progressService.Progress.ObtainModule(data, 1, false);
                        Debug.Log(data.Id);
                    }
                }
            }
        }

        public void Exit()
        {
            
        }

        private void LoadProgressOrInitNew()
        {
            _progressService.Progress =  _saveLoadService.LoadProgress() ?? GetNewProgress();
        }

        private PlayerProgress GetNewProgress()
        {
            PlayerProgress playerProgress = new PlayerProgress(0);
            
            playerProgress.StartingMachineId = 0;


            foreach (var data in _staticDataService.GetAllModules())
            {
                if (data.AvailableOnStart)
                {
                    playerProgress.AvaliableModules.Add(data.Id);
                }
            }
            
            return playerProgress;
        }
    }
}