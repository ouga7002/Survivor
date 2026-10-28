using UnityEngine;
using System.Collections;

public class KnifeAttack : MonoBehaviour
{
    [Header("参照設定（インスペクターで紐付け）")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform aimCursor;

    [Header("攻撃パラメータ")]
    [Tooltip("攻撃力（ATK: 10）")]
    [SerializeField] private float attackPower = 10f;

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
    private Collider2D knifeCollider;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        knifeCollider = GetComponent<Collider2D>();

        // 待機時は非表示＆判定もオフにしておく
        if (spriteRenderer != null) spriteRenderer.enabled = false;
        if (knifeCollider != null) knifeCollider.enabled = false;

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

    // カーソルの少し先で展開する斬撃スイングモーション ＆ 攻撃判定
    IEnumerator SwingAheadOfCursorRoutine()
    {
        isAttacking = true;

        // 攻撃の瞬間だけ「表示」し、コライダーを「有効（Enabled）」にして当たり判定を出す
        if (spriteRenderer != null) spriteRenderer.enabled = true;
        if (knifeCollider != null) knifeCollider.enabled = true;

        float elapsed = 0f;

        // プレイヤーからカーソルへの方向と距離を取得
        Vector3 dirToCursor = (aimCursor.position - player.position).normalized;
        float baseAngle = Mathf.Atan2(dirToCursor.y, dirToCursor.x) * Mathf.Rad2Deg;

        float cursorDist = Vector3.Distance(player.position, aimCursor.position);
        float knifeRadius = cursorDist + distanceAheadOfCursor;

        float startAngle = baseAngle - (swingAngleRange / 2f);
        float endAngle = baseAngle + (swingAngleRange / 2f);

        while (elapsed < swingDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / swingDuration;

            // 滑らかなイージング
            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            float currentAngle = Mathf.Lerp(startAngle, endAngle, smoothT);

            // 座標と回転を計算して適用
            float rad = currentAngle * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f) * knifeRadius;

            transform.position = player.position + offset;
            transform.rotation = Quaternion.Euler(0f, 0f, currentAngle - 45f);

            yield return null;
        }

        // 攻撃終了時に非表示＆判定もオフにする
        if (spriteRenderer != null) spriteRenderer.enabled = false;
        if (knifeCollider != null) knifeCollider.enabled = false;

        isAttacking = false;
    }

    // 敵にナイフが当たったときの処理
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 接触したオブジェクトに「Enemy」タグがついている、または名前に「enemy」が含まれている場合
        if (collision.CompareTag("Enemy") || collision.name.ToLower().Contains("enemy"))
        {
            Debug.Log($"[攻撃成功] ナイフが敵にヒット！ ATK: {attackPower} のダメージ");

            // 敵を消滅させる（キル処理）
            Destroy(collision.gameObject);
        }
    }
}