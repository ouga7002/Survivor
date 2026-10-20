using UnityEngine;
using UnityEngine.InputSystem; // 新しいInput Systemを使用

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerStatus))] // プレイヤーのステータス管理が必須
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private PlayerStatus playerStatus;
    private Vector2 moveInput;

    void Start()
    {
        // 必要なコンポーネントの参照を取得
        rb = GetComponent<Rigidbody2D>();
        playerStatus = GetComponent<PlayerStatus>();

        // 2Dゲームで物理回転しないように固定（Z軸回転を凍結）
        rb.freezeRotation = true;

        // 見下ろし型ゲームのため、重力を無効化（勝手に下に落ちないようにする）
        rb.gravityScale = 0f;
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
        // 移動速度は PlayerStatus から取得した値（初期値: 5）を使う
        rb.linearVelocity = moveInput * playerStatus.MoveSpeed;
    }
}