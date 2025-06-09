using UnityEngine;

namespace CodeBase.UI.Elements.Bars
{
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField] private Bar _bar;
        
        private Transform _player;
        private float _height;

        public void Construct(Transform playerTransform, float height)
        {
            _player = playerTransform;
            _height = height;
        }

        private void LateUpdate()
        {
            if (_player == null) return;
            _bar.SetValue(_player.transform.position.y, _height);
        }
    }
}