using System;
using System.Collections.Generic;
using CodeBase.UI.Services;
using UnityEngine;

namespace Assets.CodeBase.Infrastructure.Data.PlayerData
{
    [Serializable]
    public class TutorialData
    {
        public List<WindowType> TutorialsPassed = new List<WindowType>();

        public FirstTutorialProgress FirstTutorialProgress = new FirstTutorialProgress();

    }

    [Serializable]
    public class FirstTutorialProgress
    {
        public bool FullCompleted
        {
            get => _fullCompleted;
        }

        public bool MovementCompleted
        {
            get => _movementCompleted;
            set
            {
                _movementCompleted = value;
                if (_movementCompleted)
                {
                    OnTutorialProgressChanged(1); // Movement completed
                }
            }
        }

        public bool ModulesCompleted
        {
            get => _modulesCompleted;
            set
            {
                _modulesCompleted = value;
                if (_modulesCompleted)
                {
                    OnTutorialProgressChanged(2); // Modules completed
                }
            }
        }

        public bool CCCompleted
        {
            get => _ccCompleted;
            set
            {
                _ccCompleted = value;
                if (_ccCompleted)
                {
                    OnTutorialProgressChanged(3); // CC completed
                }
            }
        }

        [SerializeField] private bool _fullCompleted;
        [SerializeField] private bool _movementCompleted;
        [SerializeField] private bool _modulesCompleted;
        [SerializeField] private bool _ccCompleted;

        public event Action<int> TutorialProgressChanged; // Single event with an int parameter

        public void Reset()
        {
            _fullCompleted = false;
            MovementCompleted = false;
            ModulesCompleted = false;
            CCCompleted = false;
        }

        public void FullComplete()
        {
            _fullCompleted = true;
            OnTutorialProgressChanged(4); // Full completed
        }

        private void OnTutorialProgressChanged(int progressType)
        {
            TutorialProgressChanged?.Invoke(progressType);
        }
    }
}