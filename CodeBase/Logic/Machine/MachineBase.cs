using System;
using System.Collections.Generic;
using System.Linq;
using Assets.CodeBase.Infrastructure.Data;
using Assets.CodeBase.Infrastructure.Services.Input;
using Assets.CodeBase.Logic.DynamicDataLogic;
using Assets.CodeBase.Logic.DynamicDataLogic.DataHolders;
using Assets.CodeBase.Logic.Machine;
using Assets.CodeBase.StaticData;
using Assets.CodeBase.Tools;
using CodeBase.Infrastructure.Data;
using CodeBase.Logic.DynamicDataLogic;
using CodeBase.Logic.MachineModules;
using CodeBase.UI.Services.GameStopOver;
using UnityEngine;

namespace CodeBase.Logic.Machine
{
    public sealed class MachineBase : MonoBehaviour
    {
        [HideInInspector]public MachineData Data;
        
        [SerializeField] private Health _health;
        [SerializeField] private Fuel _fuel;
        
        [SerializeField] private Rigidbody2D _rigidbody;
        [SerializeField] private WheelMovement _wheelMovement;
        
        private float _rigidbodyAngularVelocity;
        private float _rotationSpeed;
        private float _currentRotation;
        private Vector2 _axis;

        public Rigidbody2D Rigidbody2D
        {
            get => _rigidbody;
        }

        public Health Health { get => _health; }
        public Fuel Fuel { get => _fuel;  }

        //public event Action Initialized;
        private IInputService _inputService;

        private float _noFuelDamageCounter = 1f;
        
        public void Construct(MachineData data, IInputService inputService)
        {
            _inputService = inputService;

            Data = data;
            
            _fuel.SetStartEnergyLevel(Data.Fuel);
        }

        public void Initialize()
        {
            SetModulesActive(true);

            //Initialized?.Invoke();
        }

        private void FixedUpdate()
        {
            //if initialized
            if (_inputService == null) return;
            
            
            _wheelMovement.Move(_axis, Data.MovingSpeed, Data.RotationSpeed);
            _axis = new Vector2(0f, 0f);
            
            if (!_fuel.Available)
            {
                if (_noFuelDamageCounter < 0f)
                {
                    foreach (var moduleInfo in Data.Modules)
                    {
                        if (moduleInfo == null) continue;

                        moduleInfo.Module.Deactivate();

                    }
                    _noFuelDamageCounter = 0.5f;
                    _health.TakeDamage(Mathf.RoundToInt(_health.Max * 0.2f));
                }
                _noFuelDamageCounter -= Time.fixedDeltaTime;
            }
            else
            {
                foreach (var moduleInfo in Data.Modules)
                {
                    if (moduleInfo != null && moduleInfo.Module != null)
                    {
                        if (moduleInfo.CanBeActivated)
                        {
                            moduleInfo.ActivationReloadTick -= Time.fixedDeltaTime;
                            if (moduleInfo.ActivationReloadTick < 0 && moduleInfo.Module.Active)
                            {
                                moduleInfo.Module.Deactivate();
                            }
                        }
                    }
                }
            }
        }

        public void Move(Vector2 axis)
        {
            float frameMovingCost = Data.MovingSpeed * Time.deltaTime;

            //if (!_fuel.HasEnough(frameMovingCost)) axis = Vector2.zero;
            if (!_fuel.Available) axis = Vector2.zero;
            
            if (axis.sqrMagnitude != 0)
            {
                _fuel.Spend(frameMovingCost);

            }
            _axis += axis;
        }

        public void HandleInputForModules(int number)
        {
            foreach (var moduleInfo in Data.Modules)
            {
                if (moduleInfo == null) continue;
                if (moduleInfo.CanBeActivated)
                {
                    if (moduleInfo.Group == number)
                    {
                        if (moduleInfo.ActivationReloadTick <
                            -moduleInfo.Module.statsHolder.Stats[StatType.ActivationReload].Value)
                        {
                            if (moduleInfo.Module.statsHolder.Stats.ContainsKey(StatType.Duration))
                            {
                                moduleInfo.ActivationReloadTick =
                                    moduleInfo.Module.statsHolder.Stats[StatType.Duration].Value;
                                moduleInfo.Module.Activate();
                            }
                            else
                            {
                                moduleInfo.ActivationReloadTick = 0f;
                                moduleInfo.Module.Activate();
                                moduleInfo.Module.Deactivate();
                            }
                        }
                    }
                }
            }
        }

        public void SetGroupActive(int group, bool active)
        {
            foreach (var moduleInfo in Data.Modules)
            {
                if (moduleInfo == null) continue;
                if (!moduleInfo.CanBeActivated) continue;

                if (group == moduleInfo.Group)
                {
                    if (active) moduleInfo.Module.Activate();
                    else moduleInfo.Module.Deactivate();
                }
            }
        }

        public Vector2 GetWorldPositionForModule(int modulePlace)
        {
            float blockDistance = GameConstants.ModuleSize + GameConstants.ModuleDistance;
            Vector2 position = (Vector2)transform.position;

            Vector2 offset = (Data.ModulePlacementInfo[modulePlace].Offset) * blockDistance;;
            return position + offset;
        }

        public void RegisterModule(MachineModule machineModule, int place,
            Direction direction, int group, bool canBeActivated, MachineModuleStaticData staticData)
        {
            machineModule.transform.parent = transform;
            machineModule.Initialize();
            Data.Modules[place] = new MachineModuleInfo
            {
                Module =  machineModule, Direction =  direction, Group = group,
                CanBeActivated = canBeActivated, StaticData = staticData,
            };
        }

        public void MoveTowards(Vector2 axis)
        {
            //Joystick
            /*
            float speed = axis.magnitude;
            //axis = transform.InverseTransformVector(axis);

            float angle = Vector2.Angle(axis, Vector2.up);
            if (angle > 45 && angle < 135)
            {
                speed = 0f;
            }
            float frameMovingCost = Data.MovingCost * Time.deltaTime;

            if (!_energy.HasEnough(frameMovingCost)) axis = Vector2.zero;
            
            if (axis.sqrMagnitude != 0)
            {
                _energy.Spend(frameMovingCost);
            }
            
            if ( Mathf.Abs(angle) > 135f)
            {
                axis = new Vector2(axis.x, axis.y.SetAbsoluteValue(speed));
            }
            _axis += axis;
            */
           
        }

        public void AdjustMaxHealth(int amount) => _health.AdjustMaxHealth(amount);

        public bool CheckIfGroupUsed(int group)
        {
            foreach (var info in Data.Modules)
            {
                if (info == null) continue;

                if (info.Group == group)
                {
                    return true;
                }
            }

            return false;
        }

        public float GetGroupReload(int group)
        {
            foreach (var info in Data.Modules)
            {
                if (info == null) continue;

                if (info.Group == group)
                {
                    
                    if (info.ActivationReloadTick > 0)
                    {
                        return (float)info.ActivationReloadTick /
                               info.Module.statsHolder.Stats[StatType.Duration].Value;
                    }
                    else
                    {
                        return (float)info.ActivationReloadTick /
                               info.Module.statsHolder.Stats[StatType.ActivationReload].Value;
                    }
                }
            }

            Debug.LogWarning("Error");
            return 0f;
        }

        public void SpeedUpCCReload(float value)
        {
            foreach (var moduleInfo in Data.Modules)
            {
                if (moduleInfo != null)
                {
                    if (moduleInfo.CanBeActivated)
                    {
                        if (moduleInfo.ActivationReloadTick < 0)
                        {
                            moduleInfo.ActivationReloadTick -= value;
                        }
                    }
                }
            }
        }

        public void SetModulesActive(bool to)
        {
            foreach (var moduleInfo in Data.Modules)
            {
                if (moduleInfo == null) continue;
                if (!moduleInfo.CanBeActivated)
                {
                    if (to) moduleInfo.Module.Activate();
                    else moduleInfo.Module.Deactivate();
                }
            }
        }
    }
}