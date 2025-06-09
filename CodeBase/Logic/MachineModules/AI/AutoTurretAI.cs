using System;
using Assets.CodeBase.Logic.AttackLogic.Activators;
using Assets.CodeBase.Logic.Enemies;
using Assets.CodeBase.Logic.Raycasters;
using UnityEngine;

namespace CodeBase.Logic.MachineModules.AI
{
    public class AutoTurretAI : MonoBehaviour
    {
        [SerializeField] private MachineModule _module;
        [SerializeField] private ArcDetector _detector;
        [SerializeField] private AttackActivator _activator;
        [SerializeField] private RotateToTarget _rotateToTarget;


        private void Update()
        {
            if (_module.Active)
            {
                if (_detector.Detected.Count > 0)
                {
                    SetTarget();
                    _activator.Active = true;
                    return;
                }
            }
            _rotateToTarget.SetTargetPoint(transform.position + transform.up);
            _activator.Active = false;
        }

        private void SetTarget()
        {
            float min = Single.MaxValue;
            foreach (var col in _detector.Detected)
            {
                float dist = (col.bounds.center - transform.position).magnitude;
                if (min > dist)
                {
                    min = dist;
                    _rotateToTarget.SetTargetPoint(col.bounds.center);
                }
            }
        }
    }
}