using Assets.CodeBase.Logic.DynamicDataLogic;
using UnityEngine;

namespace CodeBase.UI.Elements.Bars
{
    public class EnergyBar : MonoBehaviour
    {
          public Bar _bar;
          private Energy _energy;

          public void Construct(Energy energy)
          {
              _energy = energy;
              _energy.CurrentEnergyChanged += UpdateBar;
              UpdateBar();
          }

          private void SetValue(float current, float max)
          {
                _bar.SetValue(current, max);
          }

          private void UpdateBar()
          {
              SetValue(_energy.Current, _energy.Max);
          }
    }
}
