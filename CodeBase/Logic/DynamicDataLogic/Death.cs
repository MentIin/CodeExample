using System;
using System.Collections;
using UnityEngine;

namespace Assets.CodeBase.Logic.DynamicDataLogic
{
  public class Death : MonoBehaviour
  {
    public Health Health;
    [SerializeField] private float _timeToDie = 0f;
    [SerializeField] private bool _destroyOnDeath = true;

    public event Action Happen;
    public event Action StartedHappening;

    private void Start() => Health.HealthChanged += OnHealthChanged;

    private void OnDestroy() => Health.HealthChanged -= OnHealthChanged;

    private void OnHealthChanged()
    {
      if (Health.Current <= 0)
        StartCoroutine(Die());
    }

    private IEnumerator Die()
    {
      Health.HealthChanged -= OnHealthChanged;
      
      SpawnDeathFx();

      StartedHappening?.Invoke();
      yield return new WaitForSeconds(_timeToDie);
      
      Happen?.Invoke();
      if (_destroyOnDeath) Destroy(this.gameObject);

    }

    private void SpawnDeathFx()
    {
      
    }
  }
}