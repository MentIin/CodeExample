using System;
using Assets.CodeBase.Infrastructure.Services.Input;
using Assets.CodeBase.Infrastructure.Services.PersistentProgress;
using CodeBase.Logic.Machine;
using UnityEngine;

namespace Assets.CodeBase.Logic.Machine
{
    public class MachineBasePlayerController : MonoBehaviour
    {
        private MachineBase _base;
        private IInputService _inputService;
        private IPersistentProgressService _progressService;

        private void OnDestroy()
        {
            _inputService.ModulePressed -= HandleInputForModules;
        }

        public void Construct(IInputService inputService, MachineBase machineBase, IPersistentProgressService progressService)
        {
            _progressService = progressService;
            _inputService = inputService;
            _inputService.ClearInput();
            _inputService.ModulePressed += HandleInputForModules;
            _base = machineBase;
            
        }

        private void FixedUpdate()
        {
            HandleMovement();

            if (!_progressService.Progress.TutorialData.FirstTutorialProgress.FullCompleted)
            {
                if (!_progressService.Progress.TutorialData.FirstTutorialProgress.MovementCompleted)
                {
                    _base.SetModulesActive(false);
                }
                else
                {
                    _base.SetModulesActive(true);
                }
                
                if (!_progressService.Progress.TutorialData.FirstTutorialProgress.CCCompleted)
                {
                    _base.Fuel.Restore(1f);
                }
                Debug.Log("Tutorial Not complited: restrictions");
            }
        }


        private void HandleInputForModules(int number)
        {
            _base.HandleInputForModules(number);
        }

        private void HandleMovement()
        {
            if (_inputService == null) return;
            Vector2 axis = _inputService.GetAxis();
            
            _base.Move(axis);
            //_base.MoveTowards(axis);
        }
    }
}