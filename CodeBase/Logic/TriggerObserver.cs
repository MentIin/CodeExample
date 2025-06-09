using System;
using Assets.CodeBase.Infrastructure.Data;
using Assets.CodeBase.Logic.DynamicDataLogic;
using NaughtyAttributes;
using UnityEngine;

namespace Assets.CodeBase.Logic
{
    [RequireComponent(typeof(Collider2D))]
    public class TriggerObserver : MonoBehaviour
    {
        [SerializeField] private bool _checkOnlySpecificTeam=false;
        [SerializeField] [ShowIf("_checkOnlySpecificTeam")]private Team _team;
        
        
        public event Action<Collider2D> TriggerEnter;
        public event Action<Collider2D> TriggerExit;
        public event Action<Collider2D> TriggerStay;
        public int CollidersInside { get; private set; }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (_checkOnlySpecificTeam)
            {
                if (col.GetComponent<Health>()?.Team != _team)
                {
                    return;
                }
            }
            
            Debug.Log("ENTER");
            TriggerEnter?.Invoke(col);
            CollidersInside += 1;
        }

        private void OnTriggerExit2D(Collider2D col)
        {
            if (_checkOnlySpecificTeam)
            {
                if (col.GetComponent<Health>()?.Team != _team)
                {
                    return;
                }
            }
            TriggerExit?.Invoke(col);
            CollidersInside -= 1;
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            TriggerStay?.Invoke(other);
        }
    }
}