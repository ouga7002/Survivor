using UnityEngine;
using System.Collections;

public class KnifeAttack : MonoBehaviour
{
    [Header("参照設定（インスペクターで紐付け）")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform aimCursor;

    [Header("攻撃パラメータ")]
    [Tooltip("攻撃間隔（秒）")]
    [SerializeField] private float attackInterval = 2.0f;

    [Tooltip("斬撃（スイング）にかかる時間")]
    [SerializeField] private float swingDuration = 0.25f;

    [Tooltip("カーソルよりもどれくらい先（外側）で攻撃するか")]
    [SerializeField] private float distanceAheadOfCursor = 0.4f;

    [Tooltip("振る時のスイング角度幅")]
    [SerializeField] private float swingAngleRange = 90f;

    private float attackTimer = 0f;
    private bool isAttacking = false;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 待機時は非表示にしてカーソルと被らないようにする
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        // 未設定時の自動検索保険
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

        // 攻撃中でないときはタイマーを進める
        if (!isAttacking)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackInterval)
            {
                attackTimer = 0f;
                StartCoroutine(SwingAheadOfCursorRoutine());
            }
        }
    }

    // カーソルの少し先（外側）で展開する斬撃スイングモーション
    IEnumerator SwingAheadOfCursorRoutine()
    {
        isAttacking = true;

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }

        float elapsed = 0f;

        // プレイヤーからカーソルへの方向と、現在のカーソルまでの距離を取得
        Vector3 dirToCursor = (aimCursor.position - player.position).normalized;
        float baseAngle = Mathf.Atan2(dirToCursor.y, dirToCursor.x) * Mathf.Rad2Deg;

        // 「プレイヤーからカーソルまでの距離 + 先に出る分の距離」を計算
        float cursorDist = Vector3.Distance(player.position, aimCursor.position);
        float knifeRadius = cursorDist + distanceAheadOfCursor;

        // カーソルの方角を中央にして、左右にスイングする角度を計算
        float startAngle = baseAngle - (swingAngleRange / 2f);
        float endAngle = baseAngle + (swingAngleRange / 2f);

        while (elapsed < swingDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / swingDuration;

            // 滑らかなイージング
            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            float currentAngle = Mathf.Lerp(startAngle, endAngle, smoothT);

            // 計算した「先側の半径」を使って円周上の座標を算出
            float rad = currentAngle * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f) * knifeRadius;

            transform.position = player.position + offset;
            transform.rotation = Quaternion.Euler(0f, 0f, currentAngle - 45f);

            yield return null;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        isAttacking = false;
    }
}