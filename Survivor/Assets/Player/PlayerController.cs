using UnityEngine;
using UnityEngine.InputSystem; // 新しいInput Systemの名前空間

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("移動設定")]
    [Tooltip("移動速度")]
    public float moveSpeed = 5.0f;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    void Start()
    {
        // Rigidbody2Dの参照を取得
        rb = GetComponent<Rigidbody2D>();

        // 2Dゲームで物理回転しないように固定（Z軸回転を凍結）
        rb.freezeRotation = true;
    }

    void Update()
    {
        // 新しいInput System (Keyboard.current) を使ったWASD / 矢印キーの入力取得
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return; // キーボードが接続されていない場合の安全対策

        float moveX = 0f;
        float moveY = 0f;

        // 左右の入力 (A/D または 左右矢印)
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) moveX += 1f;
        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) moveX -= 1f;

        // 上下の入力 (W/S または 上下矢印)
        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) moveY += 1f;
        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) moveY -= 1f;

        // 入力値をベクトルに格納（斜め移動時の速度が速くなりすぎないようにNormalized処理）
        moveInput = new Vector2(moveX, moveY).normalized;
    }

    void FixedUpdate()
    {
        // 物理演算を用いた移動処理
        rb.linearVelocity = moveInput * moveSpeed;
    }
}