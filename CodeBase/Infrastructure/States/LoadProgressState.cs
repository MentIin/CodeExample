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
using CodeBase.Infrastructure.Services.Analytics; 
using UnityEngine;

namespace CodeBase.Infrastructure.States
{
    public class LoadProgressState : IState
    {
        private readonly GameStateMachine _stateMachine;
        private readonly IPersistentProgressService _progressService;
        private readonly ISaveLoadService _saveLoadService;
        private readonly IStaticDataService _staticDataService;
        private readonly IUserInfoService _userInfoService;
        private readonly IAnalyticsService _analyticsService;
        private readonly PauseService _pauseService;
        private readonly ICoroutineRunner _coroutineRunner;

        private Coroutine _analyticServiceTimeoutCoroutine;
        private bool _hasAnalyticsInitialized = false;
        private bool _isAnalyticsTimeoutReached = false;

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

            _progressService.Progress = _saveLoadService.LoadProgress() ?? GetNewProgress();

            FixIncorrectSaves();
            ObtainStartModules();

            _analyticsService.Initialize();
            _analyticsService.Initialized += OnAnalyticsInitialized;
            _analyticServiceTimeoutCoroutine = _coroutineRunner.StartCoroutine(AnalyticTimeoutCoroutine());
        }

        public void Exit()
        {
            _analyticsService.Initialized -= OnAnalyticsInitialized; 
            if (_analyticServiceTimeoutCoroutine != null)
            {
                _coroutineRunner.StopCoroutine(_analyticServiceTimeoutCoroutine);
                _analyticServiceTimeoutCoroutine = null;
            }
        }

        private IEnumerator AnalyticTimeoutCoroutine()
        {
            yield return new WaitForSecondsRealtime(3f);

            if (!_hasAnalyticsInitialized) 
            {
                _isAnalyticsTimeoutReached = true;
                Debug.LogWarning("Analytics initialization timed out. Proceeding without full analytics setup.");
                ProceedToNextState();
            }
        }

        private void OnAnalyticsInitialized()
        {
            _analyticsService.Initialized -= OnAnalyticsInitialized;

            if (_isAnalyticsTimeoutReached)
            {
                Debug.LogWarning("Analytics initialized AFTER timeout. Ignoring late initialization.");
                return;
            }

            _hasAnalyticsInitialized = true;
            if (_analyticServiceTimeoutCoroutine != null)
            {
                _coroutineRunner.StopCoroutine(_analyticServiceTimeoutCoroutine);
                _analyticServiceTimeoutCoroutine = null;
            }

            ProceedToNextState();
        }

        private void ProceedToNextState()
        {
            FixShopMachineModels(); 

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
            // TODO
        }

        private void FixShopMachineModels()
        {
            List<KeyValuePair<int, MachineModel>> toDelete = new List<KeyValuePair<int, MachineModel>>();
            foreach (var valuePair in _progressService.Progress.ShopMachineModels)
            {
                if (_staticDataService.ForStartingMachine(valuePair.Key) == null ||
                    valuePair.Value.MachineModules.Count != _staticDataService.ForStartingMachine(valuePair.Key).Modules.Length)
                {
                    toDelete.Add(valuePair);
                }
            }

            foreach (var keyValuePair in toDelete)
            {
                _progressService.Progress.ShopMachineModels.Remove(keyValuePair.Key);
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
                        // Debug.Log($"Obtained starting module: {data.Id}"); 
                    }
                }
            }
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