using UnityEngine;

public class GateSkill : MonoBehaviour
{
    [Header("参照設定")]
    [Tooltip("プレイヤーのトランスフォーム")]
    [SerializeField] private Transform player;

    [Tooltip("エイムカーソルのトランスフォーム")]
    [SerializeField] private Transform aimCursor;

    [Header("シーン上の2つのゲートを直接アタッチ")]
    [SerializeField] private Transform gate1;
    [SerializeField] private Transform gate2;

    [Header("パラメータ")]
    [Tooltip("プレイヤーから後ろ側の配置距離")]
    [SerializeField] private float backDistance = 1.8f;

    [Tooltip("2つの輪を配置する際の間隔")]
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

        // 接続確認のログ
        Debug.Log($"[GateSkill] Player: {(player != null ? player.name : "null")} | AimCursor: {(aimCursor != null ? aimCursor.name : "null")}");
    }

    void Update()
    {
        if (player == null || aimCursor == null)
        {
            // 何が足りていないかコンソールに表示
            Debug.LogWarning($"[GateSkill] 参照切れ -> Player: {player}, AimCursor: {aimCursor}");
            return;
        }

        // 1. プレイヤーからカーソルへの方向ベクトルを算出
        Vector3 dirToCursor = (aimCursor.position - player.position);

        // デフォルトの反対方向
        Vector3 oppositeDir = Vector3.left;
        if (dirToCursor.sqrMagnitude > 0.001f)
        {
            oppositeDir = -dirToCursor.normalized;
        }

        // 2. プレイヤーの位置を基準にした中心座標
        Vector3 centerPos = player.position + (oppositeDir * backDistance);

        // 3. 垂直ベクトルの計算（2Dの左右振り分け用）
        Vector3 perpendicular = new Vector3(-oppositeDir.y, oppositeDir.x, 0f);

        // 4. 2つのゲートの位置と回転を更新
        if (gate1 != null)
        {
            gate1.position = centerPos + (perpendicular * (spreadSpacing / 2f));
            SetRotation(gate1, oppositeDir);
        }

        if (gate2 != null)
        {
            gate2.position = centerPos - (perpendicular * (spreadSpacing / 2f));
            SetRotation(gate2, oppositeDir);
        }
    }

    void SetRotation(Transform gateTransform, Vector3 dir)
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        gateTransform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}