using System;
using Assets.CodeBase.Logic.AttackLogic.Activators;
using Assets.CodeBase.Logic.DynamicDataLogic.DataHolders;
using Assets.CodeBase.Logic.Enemies;
using Assets.CodeBase.Logic.Follow;
using CodeBase.Logic.Follow;
using UnityEngine;

namespace Assets.CodeBase.Logic.AI.Enemies
{
    public class ArcherAI : MonoBehaviour
    {
        [SerializeField] private MobDataHolder _dataHolder;
        [SerializeField] private CheckIfSeeBounds _checkIfSeeBounds;
        [SerializeField] private CheckableFollowTarget _follow;
        [SerializeField] private AttackActivator _activator;
        [SerializeField] private FreeMove _freeMove;

        [SerializeField] private bool _shootIfInRange; 
        
        private Transform _target;
        private void Start()
        {
            _target = _dataHolder.PlayerMachineBase.transform;
        }

        private void Update()
        {
            if (_checkIfSeeBounds.See)
            {
                _freeMove.enabled = false;
                _activator.Active = true;
                if (_shootIfInRange)
                {
                    if (_checkIfSeeBounds.Distance > _dataHolder.Stats[StatType.Range].Value)
                    {
                        _activator.Active = false;
                    }
                }

                if (_target)
                {
                    Vector2 targetPoint = _target.position;
                    if (_follow)
                    {
                        _follow.enabled = true;
                        _follow.SetTargetPoint(_checkIfSeeBounds.Point);
                    }
                }
            }
            else
            {
                _activator.Active = false;
                if (_follow.IsReached())
                {
                    _freeMove.enabled = true;
                }
                
            }
        }
    }
}