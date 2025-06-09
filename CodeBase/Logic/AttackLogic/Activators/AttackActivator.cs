using System;
using Assets.CodeBase.Infrastructure.Data;
using Assets.CodeBase.Logic.DynamicDataLogic.DataHolders;
using NaughtyAttributes;
using UnityEngine;

namespace Assets.CodeBase.Logic.AttackLogic.Activators
{
    public class AttackActivator : MonoBehaviour
    {
        [Required()]public BaseDataHolder DataHolder;
        public Attack[] Attacks;
        public virtual bool Active { get; set; }
        protected float reloadTime => DataHolder.Stats[StatType.ReloadTime].Value.FromMillisecondsToSeconds();

        protected float timePass;

        public event Action Done;
        public event Action ReadyToBeDone;

        protected virtual void Update()
        {
            timePass += Time.deltaTime;
            if (Active && timePass >= reloadTime)
            {
                if (CheckActiveAndEnergy())
                {
                    timePass = 0f;
                    foreach (var attack in Attacks)
                    {
                        attack.Activate();
                    }
                }
            }
        }

        protected bool CheckActiveAndEnergy()
        {
            InvokeEnergyCheck();
            if (Active)
            {
                InvokeDone();
                return true;
            }
            return false;
        }

        protected void InvokeEnergyCheck()
        {
            ReadyToBeDone?.Invoke();
        }

        protected void InvokeDone()
        {
            Done?.Invoke();
        }
    }
}