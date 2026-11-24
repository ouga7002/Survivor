
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerStatus))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private PlayerStatus playerStatus;
    private Vector2 moveInput;
    private SpriteRenderer spriteRenderer;
    private Animator animator; // 追加

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerStatus = GetComponent<PlayerStatus>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>(); // 追加

        rb.freezeRotation = true;
        rb.gravityScale = 0f;
    }

    void Update()
    {
        Gamepad gamepad = Gamepad.current;

        float moveX = 0f;
        float moveY = 0f;

        if (gamepad != null)
        {
            moveX = gamepad.leftStick.x.ReadValue();
            moveY = gamepad.leftStick.y.ReadValue();
        }
        else
        {
            Keyboard keyboard = Keyboard.current;

            if (keyboard != null)
            {
                if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) moveX += 1f;
                if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) moveX -= 1f;
                if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) moveY += 1f;
                if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) moveY -= 1f;
            }
        }

        moveInput = new Vector2(moveX, moveY).normalized;

        // 歩行アニメーションの切り替え
        if (animator != null)
        {
            animator.SetBool("IsMoving", moveInput.sqrMagnitude > 0.01f);
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveInput * playerStatus.MoveSpeed;

        if (spriteRenderer != null)
        {
            if (rb.linearVelocity.x > 0.01f)
            {
                spriteRenderer.flipX = false;
            }
            else if (rb.linearVelocity.x < -0.01f)
            {
                spriteRenderer.flipX = true;
            }
        }
    }
}
