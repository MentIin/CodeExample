using Assets.CodeBase.Infrastructure.Data;
using Assets.CodeBase.Logic.DynamicDataLogic.DataHolders;
using UnityEngine;

namespace Assets.CodeBase.Logic.AttackLogic.Activators
{
    public class FocusedAttackActivator : AttackActivator
    {
        private float _currentReloadTime;
        private float _focusTime => DataHolder.Stats[StatType.FocusTime].Value.FromMillisecondsToSeconds();
        private float _reloadMinSpeed => DataHolder.Stats[StatType.ReloadTime].Value.FromMillisecondsToSeconds();
        private float _currentFocus;
        public float CurrentFocus
        {
            get => _currentFocus;
        }
        
        protected override void Update()
        {
            timePass += Time.deltaTime;

            UpdateFocus();
            if (_currentFocus > 0)
            {
                _currentReloadTime = _reloadMinSpeed / _currentFocus;
            }
            
            ShootIfActive();
        }

        private void UpdateFocus()
        {
            if (Active)
            {
                _currentFocus += Time.deltaTime / _focusTime;
                if (_currentFocus > 1f) _currentFocus = 1f;
            }
            else
            {
                _currentFocus -= Time.deltaTime / _focusTime;
                if (_currentFocus < 0f) _currentFocus = 0f;
            }
        }

        private void ShootIfActive()
        {
            if (Active && timePass >= _currentReloadTime)
            {
                timePass = 0f;
                
                if (CheckActiveAndEnergy())
                {
                    foreach (var attack in Attacks)
                    {
                        attack.Activate();
                    }
                }
            }
        }
    }
}