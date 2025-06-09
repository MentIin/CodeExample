using System;
using Assets.CodeBase.Logic.DynamicDataLogic;
using UnityEngine;

namespace CodeBase.UI.Elements.Bars
{
    public class HpBar : MonoBehaviour
    {
          public Bar _bar;
          private Health _health;
          private RectTransform _rectTransform;
        

          public void Construct(Health health)
          {
              _rectTransform = GetComponent<RectTransform>();
              _health = health;

              Initialize();
          }

          private void Initialize()
          {
              _health.HealthChanged += UpdateBar;
              _bar.Initialize();
              UpdateBar();
          }

          private void LateUpdate()
          {
              Vector3 offset = Vector3.down * 5f;
              if (_health == null) return;
              Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main,
                  _health.transform.position + offset);
              _rectTransform.position = screenPoint;
          }



          private void SetValue(float current, float max)
          {
                
                _bar.SetValue(current, max);
          }

          private void UpdateBar()
          {
              if (_health) SetValue(_health.Current, _health.Max);
          }
    }
}
