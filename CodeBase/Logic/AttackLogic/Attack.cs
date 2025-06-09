using System;
using Assets.CodeBase.Logic.DynamicDataLogic;
using Assets.CodeBase.Logic.DynamicDataLogic.DataHolders;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace Assets.CodeBase.Logic.AttackLogic
{
    public abstract class Attack : MonoBehaviour
    {
        [Required()] public BaseDataHolder DataHolder;
        [FormerlySerializedAs("_dontDamageOwner")] [SerializeField] private bool _dontDamageOwnerTeam = true;
        [HideInInspector]public Health OwnerHealth;
        public int Damage => DataHolder.Stats[StatType.Damage].Value;
        public float Range => DataHolder.Stats[StatType.Range].Value;
        public event Action Done;

        public void Activate()
        {
            OnAttack();
            Done?.Invoke();
        }
        protected abstract void OnAttack();

        protected void DealDamage(Health health, int damage)
        {
            if (health.Team != OwnerHealth.Team || !_dontDamageOwnerTeam) health.TakeDamage(damage);
        }
    }
}