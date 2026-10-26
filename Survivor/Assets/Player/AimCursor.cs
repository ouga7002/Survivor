using UnityEngine;
using UnityEngine.InputSystem; // 新しいInput Systemを使用

public class AimCursor : MonoBehaviour
{
    [Header("設定")]
    [Tooltip("プレイヤーのトランスフォームをアタッチ")]
    [SerializeField] private Transform player;

    [Tooltip("プレイヤーのまわりを回る距離（半径）")]
    [SerializeField] private float radius = 1.5f;

    // スティックを離したときも、最後に向いていた方向を維持する変数
    private Vector2 aimDirection = Vector2.right;

    void Update()
    {
        // プレイヤーが消滅していたり設定されていない場合は処理しない
        if (player == null) return;

        Gamepad gamepad = Gamepad.current;
        Vector2 stickInput = Vector2.zero;

        if (gamepad != null)
        {
            // Xboxコントローラーの右スティックの入力を取得
            stickInput = gamepad.rightStick.ReadValue();
        }
        else
        {
            // 【テスト用】コントローラーが接続されていない場合、矢印キーでも動かせるようにする保険
            Keyboard kb = Keyboard.current;
            if (kb != null)
            {
                if (kb.upArrowKey.isPressed) stickInput.y += 1f;
                if (kb.downArrowKey.isPressed) stickInput.y -= 1f;
                if (kb.leftArrowKey.isPressed) stickInput.x -= 1f;
                if (kb.rightArrowKey.isPressed) stickInput.x += 1f;
            }
        }

        // 右スティックが倒されているときだけ、向き（ベクトル）を更新する
        if (stickInput.sqrMagnitude > 0.01f)
        {
            aimDirection = stickInput.normalized;
        }

        // プレイヤーの位置を基準にして、360度どこにでもカーソルを配置する計算
        Vector3 offset = new Vector3(aimDirection.x, aimDirection.y, 0f) * radius;
        transform.position = player.position + offset;
    }
}