using CodeBase.Logic;
using UnityEngine;

namespace Assets.CodeBase.Logic.Effects
{
    public class Knocknack : MonoBehaviour
    {
        private Vector2 _force;
        private Rigidbody2D _rigidBody;
        private float _left = .2f;

        private Resistance _resistance;

        public void Construct(Vector2 force, Rigidbody2D rigidbody2D)
        {
            if (rigidbody2D.gameObject.layer == GameConstants.MapBlocksLayer)
            {
                Destroy(this);
                return;
            }
            _force = force;
            _rigidBody = rigidbody2D;

            if (rigidbody2D.TryGetComponent(out _resistance))
            {
                _force = _force.normalized * _force.magnitude * (1 - _resistance.Knockback);
                _left = _left * (1 - _resistance.Knockback);
            }
        }

        private void FixedUpdate()
        {
            if (_left <= 0)
            {
                Destroy(this);
                return;
            }

            if (_rigidBody == null)
            {
                return;;
            }
            _left -= Time.fixedDeltaTime;


            if (_resistance != null)
            {
                if (_resistance.Flying)
                {
                    _rigidBody.linearVelocity = _rigidBody.linearVelocity + _force * Time.deltaTime * 3;
                    return;
                }
            }
            
            _rigidBody.MovePosition(_rigidBody.gameObject.transform.position + 
                                    (Vector3)_force * Time.deltaTime);
        }
    }
}