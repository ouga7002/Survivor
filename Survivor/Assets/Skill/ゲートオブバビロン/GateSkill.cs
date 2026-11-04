using UnityEngine;

public class GateSkill : MonoBehaviour
{
    [Header("参照設定")]
    [Tooltip("プレイヤーのトランスフォーム")]
    [SerializeField] private Transform player;

    [Tooltip("エイムカーソルのトランスフォーム")]
    [SerializeField] private Transform aimCursor;

    [Header("配置する2つのゲート（シーン上のオブジェクトを直接アタッチ）")]
    [SerializeField] private Transform gate1;
    [SerializeField] private Transform gate2;

    [Header("パラメータ")]
    [Tooltip("プレイヤーから後ろ側の配置距離")]
    [SerializeField] private float backDistance = 1.8f;

    [Tooltip("2つのゲートの間隔")]
    [SerializeField] private float spreadSpacing = 0.8f;

    void Start()
    {
        // 自動検索保険
        if (player == null)
        {
            GameObject pObj = GameObject.Find("player2_0");
            if (pObj != null) player = pObj.transform;
        }

        if (aimCursor == null)
        {
            GameObject cObj = GameObject.Find("AimCursor");
            if (cObj != null) aimCursor = cObj.transform;
        }
    }

    void Update()
    {
        if (player == null || aimCursor == null) return;

        // 1. プレイヤーからカーソルへの方向ベクトルを算出
        Vector2 dirToCursor = (aimCursor.position - player.position).normalized;

        // 2. その「反対側（逆方向）」の基準位置を計算
        Vector2 oppositeDir = -dirToCursor;
        Vector3 centerPos = player.position + (Vector3)(oppositeDir * backDistance);

        // 3. 輪の並び方向（垂直ベクトル）を算出
        Vector2 perpendicular = new Vector2(-oppositeDir.y, oppositeDir.x);

        // 4. 配置した2つのゲートの位置と向きを動かす
        if (gate1 != null)
        {
            gate1.position = centerPos + (Vector3)(perpendicular * (-spreadSpacing / 2f));
            SetRotation(gate1, oppositeDir);
        }

        if (gate2 != null)
        {
            gate2.position = centerPos + (Vector3)(perpendicular * (spreadSpacing / 2f));
            SetRotation(gate2, oppositeDir);
        }
    }

    // 向きを合わせるヘルパーメソッド
    void SetRotation(Transform target, Vector2 dir)
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        target.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}