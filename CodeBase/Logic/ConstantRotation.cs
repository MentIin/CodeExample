using UnityEngine;

namespace Assets.CodeBase.Logic
{
    public class ConstantRotation : MonoBehaviour
    {
        [SerializeField] private float _speed;

        private void Update()
        {
            transform.Rotate(0, 0, _speed * Time.deltaTime);
        }
    }
}