using Assets.CodeBase.Logic.DynamicDataLogic.DataHolders;
using Assets.CodeBase.Logic.Enemies;
using Assets.CodeBase.Logic.Follow;
using CodeBase.Logic.Follow;
using UnityEngine;

namespace Assets.CodeBase.Logic.AI.Enemies
{
    public class ZombieMoveAI : MonoBehaviour
    {
        [SerializeField] private MobDataHolder _dataHolder;
        [SerializeField] private CheckIfSeeBounds _checkIfSeeBounds;
        [SerializeField] private CheckableFollowTarget _follow;
        [SerializeField] private FreeMove _freeMove;
        

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
                //_follow.enabled = false;
                if (_follow.IsReached())
                {
                    _freeMove.enabled = true;
                }
                
            }
        }
    }
}