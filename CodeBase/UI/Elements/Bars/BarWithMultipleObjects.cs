using UnityEngine;

namespace CodeBase.UI.Elements.Bars
{
    public class BarWithMultipleObjects : Bar
    {
        private const string Active = "active";
        [SerializeField] private GameObject _elementPrefab;
        private int _heartsAmount = 10;
        private Animator[] _animators;

        public override void Initialize()
        {
            
        }

        public void SetObjectAmount(int amount)
        {
            if (_animators != null)
            {
                foreach (var animator in _animators)
                {
                    Destroy(animator.gameObject);
                }
            }
            
            _heartsAmount = amount;
            _animators = new Animator[_heartsAmount];
              
            for (int i = 0; i < _heartsAmount; i++)
            {
                Animator animator = Instantiate(_elementPrefab, transform).GetComponent<Animator>();
                _animators[i] = animator;
            }
        }
        
        public override void SetValue(float current, float max)
        {
            int active = Mathf.CeilToInt(current / max * _heartsAmount);
            
            for (int i = 0; i < _heartsAmount; i++)
            {
                if (i < active)
                {
                    _animators[i].SetBool(Active, true);
                }
                else
                {
                    _animators[i].SetBool(Active, false);
                }
            }
        }
    }
}