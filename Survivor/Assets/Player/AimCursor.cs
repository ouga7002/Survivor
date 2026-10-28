using UnityEngine;
using UnityEngine.InputSystem;

public class AimCursor : MonoBehaviour
{
    [Header("設定")]
    [Tooltip("プレイヤーのまわりを回る距離（半径）")]
    [SerializeField] private float radius = 1.5f;

    private Transform player;
    private Vector2 aimDirection = Vector2.right;

    void Start()
    {
        // ヒエラルキー上の名前「player2_0」を検索
        GameObject playerObj = GameObject.Find("player2_0");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            GameObject taggedPlayer = GameObject.FindWithTag("Player");
            if (taggedPlayer != null)
            {
                player = taggedPlayer.transform;
            }
            else
            {
                Debug.LogError("【AimCursor】プレイヤーが見つかりませんでした！");
            }
        }
    }

    void Update()
    {
        if (player == null) return;

        Gamepad gamepad = Gamepad.current;
        Vector2 stickInput = Vector2.zero;

        if (gamepad != null)
        {
            stickInput = gamepad.rightStick.ReadValue();
        }
        else
        {
            // 【テスト用】コントローラー未接続時の矢印キー操作
            Keyboard kb = Keyboard.current;
            if (kb != null)
            {
                if (kb.upArrowKey.isPressed) stickInput.y += 1f;
                if (kb.downArrowKey.isPressed) stickInput.y -= 1f;
                if (kb.leftArrowKey.isPressed) stickInput.x -= 1f;
                if (kb.rightArrowKey.isPressed) stickInput.x += 1f;
            }
        }

        // スティックが倒されているときだけ向きを更新
        if (stickInput.sqrMagnitude > 0.01f)
        {
            aimDirection = stickInput.normalized;
        }

        // 1. プレイヤーの位置を基準にしてカーソルを円周上に配置
        Vector3 offset = new Vector3(aimDirection.x, aimDirection.y, 0f) * radius;
        transform.position = player.position + offset;

        // 2. スティックの方向（角度）を計算して、カーソル（三角形）をその方向に向かわせる
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        // ※もし三角形の先端が「上向き」で作られている場合は、angle - 90f に調整してください
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }
}