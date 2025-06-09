using Assets.CodeBase.Logic.AttackLogic.Activators;

namespace CodeBase.Logic.AttackLogic.Activators
{
    public class OneTimeAttackActivator : AttackActivator
    {
        public override bool Active
        {
            get => false;
            set
            {
                if (value == true)
                {
                    InvokeDone();

                    foreach (var attack in Attacks)
                    {
                        attack.Activate();
                    }

                }
            }
        }
        

    }
}