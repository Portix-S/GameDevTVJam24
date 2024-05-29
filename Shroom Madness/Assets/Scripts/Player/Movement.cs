using Service;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class Movement : MonoBehaviour
    {
        [SerializeField] float _maxSpeed = 10;
        [SerializeField] float _acceleration = 1;
        [SerializeField] float _jumpForce = 5;
        
        Rigidbody _rigidbody;
        
        PlayerInputs _playerInputs;
        
        void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.maxLinearVelocity = _maxSpeed;

            if (_playerInputs == null)
            {
                _playerInputs = ServiceProvider.Get<InputService>().PlayerInputs;
            }
            //_playerInputs.Movement.Direction.performed += Move;
            _playerInputs.Movement.Jump.performed += Jump;
            
        }

        void Jump(InputAction.CallbackContext obj)
        {
            _rigidbody.AddForce(transform.up * _jumpForce, ForceMode.Impulse);
        }

        void OnEnable()
        {
            _playerInputs.Movement.Enable();
        }

        void OnDisable()
        {
            _playerInputs.Movement.Disable();
        }

        void Update()
        {
            Move(_playerInputs.Movement.Direction.ReadValue<Vector2>());
        }

        void Move(Vector2 inputDirection)
        {
            var movementDirection = new Vector3(inputDirection.x, 0f, inputDirection.y);
            _rigidbody.AddForce(movementDirection * _acceleration);
        }

        void OnDestroy()
        {
            //_playerInputs.Movement.Direction.performed -= Move;
        }
    }
}
