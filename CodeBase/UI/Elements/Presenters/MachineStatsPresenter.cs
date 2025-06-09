using UnityEngine;

namespace CodeBase.UI.Elements.Presenters
{
    public class MachineStatsPresenter : MonoBehaviour
    {
        [SerializeField] private StatPresenter _health;
        [SerializeField] private StatPresenter _speed;
        [SerializeField] private StatPresenter _energy;
        [SerializeField] private StatPresenter _energyRegeneration;

        public void Construct(int hp, int speed, int energy, int regen)
        {
            _health.Present(hp);
            _speed.Present(speed);
            _energy.Present(energy);
            _energyRegeneration.Present(regen);
        }
    }
}