using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class Movement : MonoBehaviour
    {
        [SerializeField] float _maxSpeed = 10;
        [SerializeField] float _acceleration = 1;
        
        [Header("Jump parameters")]
        [SerializeField] float _jumpForce = 5;
        [SerializeField] LayerMask _jumpSurfacesLayer;
        [SerializeField] SphereCollider _baseCollider;
        [SerializeField, Range(0.01f, 0.99f)] float _groundedScale = 0.9f;
        [SerializeField] float _groundedOffset = 0.5f;

        Vector3 _movementDirection;

        readonly RaycastHit[] _groundedHits = new RaycastHit[3];
        Vector3? _lastGroundNormal;
        float _isGroundedRadius;
        
        Rigidbody _rigidbody;

        void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.maxLinearVelocity = _maxSpeed;
            
            _isGroundedRadius = _baseCollider.radius * _baseCollider.transform.lossyScale.y *
                                _groundedScale;
        }

        public void Jump(InputAction.CallbackContext obj)
        {
            if (!obj.performed) return;

            _lastGroundNormal = IsGrounded();
            if (_lastGroundNormal == null) return;
            
            _rigidbody.AddForce(_lastGroundNormal.Value * _jumpForce, ForceMode.Impulse);
        }

        public void ReadMovementDirection(InputAction.CallbackContext obj)
        {
            Vector2 inputDirection = obj.ReadValue<Vector2>();
            _movementDirection = new Vector3(inputDirection.x, 0f, inputDirection.y);
        }

        void FixedUpdate()
        {
            Move();
        }

        void Move()
        {
            _rigidbody.AddForce(_movementDirection * _acceleration);
        }

        Vector3? IsGrounded()
        {
            int hitsSize = Physics.SphereCastNonAlloc(transform.position,
                _isGroundedRadius, Vector3.down, _groundedHits,
                _groundedOffset, _jumpSurfacesLayer, QueryTriggerInteraction.Ignore);
            if (hitsSize == 0)
            {
                return null;
            }

            RaycastHit firstHit = _groundedHits[0];
            return firstHit.normal;
        }
        
#if UNITY_EDITOR
        void Update()
        {
            _isGroundedRadius = _baseCollider.radius * _baseCollider.transform.lossyScale.y *
                                _groundedScale;
        }

        [SerializeField] bool _drawGizmos = true;
        void OnDrawGizmos()
        {
            if (!_drawGizmos) return;
            
            Gizmos.color = Color.red * 0.5f;
            
            Gizmos.DrawSphere(transform.position + Vector3.down * _groundedOffset,
                _isGroundedRadius);

            if (_lastGroundNormal != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(
                    transform.position - Vector3.Scale(_lastGroundNormal.Value,
                        _baseCollider.radius * _baseCollider.transform.lossyScale),
                    transform.position + _lastGroundNormal.Value * 10);
            }
        }
#endif
    }
}
