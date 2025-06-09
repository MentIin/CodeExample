using System;
using Assets.CodeBase.Infrastructure.Services.PersistentProgress;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.UI.Elements.Presenters.Machine
{
    public class ChangeMachineButton : MonoBehaviour
    {
        public Button Button;
        public ChangeType ChangeType;
        
        private IPersistentProgressService _progressService;

        public void Construct(IPersistentProgressService progressService)
        {
            _progressService = progressService;
            _progressService.Progress.MachineChanged += ProgressOnMachineChanged;
            
            Button.onClick.AddListener(OnClick);
            UpdateButtonActivity();
        }

        private void OnDestroy()
        {
            _progressService.Progress.MachineChanged -= ProgressOnMachineChanged;
        }

        private void ProgressOnMachineChanged()
        {
            UpdateButtonActivity();
        }

        private void UpdateButtonActivity()
        {
            if (ChangeType == ChangeType.Left)
            {
                Button.gameObject.SetActive(_progressService.Progress.CurrentMachineIndex > 0);
            }
            else
            {
                Button.gameObject.SetActive(
                    _progressService.Progress.CurrentMachineIndex < _progressService.Progress.PlayerMachines.Count-1);
            }
            
        }

        private void OnClick()
        {
            _progressService.Progress.ChangeMachine(ChangeType);
        }
    }

    public enum ChangeType
    {
        Right=1, Left=2
    }
}