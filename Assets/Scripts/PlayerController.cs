using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Basic 2D player controller with WASD movement.
/// Attach this script to your player GameObject which should have a Rigidbody2D component
/// and either a BoxCollider2D or other collider for collision detection.
/// Uses the new Input System package.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    public float moveSpeed = 5f;

    public Rigidbody2D rb;
    private Vector2 moveInput;

    public List<WeaponBase> Weapons;

    public static PlayerController gPlayer;

    public static PlayerController GetPlayer()
    {
        return gPlayer;
    }

    private void Start()
    {
        Weapons.AddRange(GetComponents<WeaponBase>());
        gPlayer = this;
        
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError("PlayerController requires a Rigidbody2D component on the GameObject!");
        }

        // Ensure the player has a collider for physics
        Collider2D collider = GetComponent<Collider2D>();
        if (collider == null)
        {
            Debug.LogWarning("PlayerController: No Collider2D found. Adding BoxCollider2D for testing.");
            gameObject.AddComponent<BoxCollider2D>();
        }

        // Ensure Rigidbody2D is configured for top-down 2D movement
        if (rb != null)
        {
            rb.gravityScale = 0;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    private void Update()
    {
        // Read input from the new Input System
        Keyboard keyboard = Keyboard.current;
        Gamepad gamepad = Gamepad.current;

        moveInput = Vector2.zero;

        // Keyboard input
        if (keyboard != null)
        {
            if (keyboard.wKey.isPressed)
                moveInput.y += 1;
            if (keyboard.sKey.isPressed)
                moveInput.y -= 1;
            if (keyboard.dKey.isPressed)
                moveInput.x += 1;
            if (keyboard.aKey.isPressed)
                moveInput.x -= 1;
        }

        // Gamepad/Controller input (left stick)
        if (gamepad != null)
        {
            moveInput += gamepad.leftStick.ReadValue();
        }
    }

    private void FixedUpdate()
    {
        if (rb != null)
        {
            // Apply movement
            rb.linearVelocity = moveInput.normalized * moveSpeed;
        }
    }
}
