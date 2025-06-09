using Assets.CodeBase.Logic.AI.Enemies;
using Assets.CodeBase.Logic.DynamicDataLogic.DataHolders;
using Assets.CodeBase.Logic.Follow;
using Assets.CodeBase.Logic.Raycasters;
using CodeBase.Logic.Follow;
using UnityEngine;

namespace Assets.CodeBase.Logic.AI.Machine
{
    public class MushroomBossAI : MonoBehaviour
    {
        [SerializeField] private MobDataHolder _dataHolder;
        [SerializeField] private MachineBaseReferenceHolder _referenceHolder;
        [SerializeField] private ArcDetector _detector;
        [SerializeField] private FreeMove _freeMove;
        [SerializeField] private CheckableFollowTarget _freeMoveFollowCheck;
        
        [Header("Poison")] [SerializeField] private BaseFollowTarget _followTargetPoison;
        [Header("Boom")][SerializeField] private BaseFollowTarget _followTargetBoom;


        private float _leftToChangeWeapon;
        private float _leftToChangePosition;
        private float _changeWeaponReload = 20f;
        private float _sleepLeft = 4f;

        private BossState _currentState;
        
        

        private void Update()
        {
            if (_sleepLeft > 0)
            {
                _sleepLeft -= Time.deltaTime;
                return;
            }
            
            
            _leftToChangeWeapon -= Time.deltaTime;
            _leftToChangePosition -= Time.deltaTime;
            if (_leftToChangePosition <= 0)
            {
                _leftToChangePosition = 10f;
                _currentState = BossState.Moving;
                _freeMove.SetTargetPoint();
                
                _freeMove.gameObject.SetActive(true);
            }
            if (_currentState == BossState.Moving && _freeMoveFollowCheck.IsReached())
            {
                _currentState = BossState.Boom;
                _freeMove.gameObject.SetActive(false);
            }
            
            
            
            if (_leftToChangeWeapon <= 0 && _currentState != BossState.Moving)
            {
                SwitchWeapon();
                _leftToChangeWeapon += _changeWeaponReload;
            }
            SetFollowTypeToCurrentWeapon();
            if (_detector.Detected.Count > 0)
            {
                CanSeeUpdate();
            }
            else
            {
                CanNotSeeUpdate();
            }
        }

        private void SwitchWeapon()
        {
            if (_currentState == BossState.Boom) _currentState = BossState.Poison;
            else if (_currentState == BossState.Poison) _currentState = BossState.Boom;
        }

        private void CanNotSeeUpdate()
        {
            
        }

        private void CanSeeUpdate()
        {
            
            _followTargetBoom.SetTargetPoint(_dataHolder.PlayerMachineBase.transform.position);
            _followTargetPoison.SetTargetPoint(_dataHolder.PlayerMachineBase.transform.position);
            
            
        }

        private void SetFollowTypeToCurrentWeapon()
        {
            if (_currentState == BossState.Boom)
            {
                _referenceHolder.MachineBase.SetGroupActive(0, true);
                _referenceHolder.MachineBase.SetGroupActive(1, false);
                _followTargetBoom.enabled = true;
                _followTargetPoison.enabled = false;
            }
            else if (_currentState == BossState.Poison)
            {
                _referenceHolder.MachineBase.SetGroupActive(1, true);
                _referenceHolder.MachineBase.SetGroupActive(0, false);
                _followTargetBoom.enabled = false;
                _followTargetPoison.enabled = true;
            }else if (_currentState == BossState.Moving)
            {
                _followTargetBoom.enabled = false;
                _followTargetPoison.enabled = false;
                
            }
        }
    }

    internal enum BossState
    {
        Poison=1, Boom=0, Moving = 10,
    }
}