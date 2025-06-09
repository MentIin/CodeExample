using CodeBase.Logic.MachineModules;
using UnityEngine;

namespace Assets.CodeBase.Logic.AttackLogic.Activators.Modules
{
    [RequireComponent(typeof(AttackActivator))]
    public class ModuleAttackActivatorActivator : MonoBehaviour
    {
        public MachineModule Module;
        public AttackActivator Activator;

        private void Awake()
        {
            Module.Activated += ModuleOnActivated;
            Module.Deactivated += ModuleOnDeactivated;
        }

        private void ModuleOnDeactivated()
        {
            Activator.Active = false;
        }

        private void ModuleOnActivated()
        {
            Activator.Active = true;
        }
        
    }
}