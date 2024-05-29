using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementTest : MonoBehaviour
{
    private Vector3 moveDirection;
    private Rigidbody rb;
    [SerializeField] private float speed = 5f;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        rb.useGravity = true;
    }
    
    private void OnDisable()
    {
        rb.useGravity = false;
    }

    public void Move(InputAction.CallbackContext context)
    {
        Vector2 direction = context.ReadValue<Vector2>();
        moveDirection = new Vector3(direction.x, 0, direction.y);
    }
    
    private void FixedUpdate()
    {
        rb.velocity = moveDirection * (speed * Time.fixedDeltaTime);
    }
}
