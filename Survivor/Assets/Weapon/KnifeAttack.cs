using UnityEngine;
using System.Collections;

public class KnifeAttack : MonoBehaviour
{
    [Header("参照設定")]
    [SerializeField] private Transform aimCursor;
    [SerializeField] private GameObject knifeVisualObject; // ナイフの見た目と判定を持つ子オブジェクト（またはプレハブ）

    [Header("攻撃パラメータ")]
    [Tooltip("ナイフ自体の基本攻撃力")]
    [SerializeField] private float baseWeaponAtk = 3f;

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
    private PlayerStatus playerStatus;
    private SpriteRenderer knifeSpriteRenderer;
    private Collider2D knifeCollider;

    void Start()
    {
        playerStatus = GetComponent<PlayerStatus>();

        if (aimCursor == null)
        {
            GameObject cObj = GameObject.Find("AimCursor");
            if (cObj != null) aimCursor = cObj.transform;
        }

        // 銃と同じように、最初はナイフのビジュアルを非表示＆判定オフにしておく
        if (knifeVisualObject != null)
        {
            knifeSpriteRenderer = knifeVisualObject.GetComponent<SpriteRenderer>();
            knifeCollider = knifeVisualObject.GetComponent<Collider2D>();

            if (knifeSpriteRenderer != null) knifeSpriteRenderer.enabled = false;
            if (knifeCollider != null) knifeCollider.enabled = false;
            knifeVisualObject.SetActive(false);
        }
    }

    void Update()
    {
        if (aimCursor == null) return;

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
        if (knifeVisualObject == null || aimCursor == null) yield break;

        isAttacking = true;
        knifeVisualObject.SetActive(true);

        // 攻撃の瞬間だけ「表示」し、コライダーを「有効」にする
        if (knifeSpriteRenderer != null) knifeSpriteRenderer.enabled = true;
        if (knifeCollider != null) knifeCollider.enabled = true;

        float elapsed = 0f;

        while (elapsed < swingDuration)
        {
            if (aimCursor == null) break;

            elapsed += Time.deltaTime;
            float t = elapsed / swingDuration;

            // プレイヤーからカーソルへの方向と距離を取得
            Vector3 dirToCursor = (aimCursor.position - transform.position).normalized;
            float baseAngle = Mathf.Atan2(dirToCursor.y, dirToCursor.x) * Mathf.Rad2Deg;

            float cursorDist = Vector3.Distance(transform.position, aimCursor.position);
            float knifeRadius = cursorDist + distanceAheadOfCursor;

            float startAngle = baseAngle - (swingAngleRange / 2f);
            float endAngle = baseAngle + (swingAngleRange / 2f);

            // 滑らかなイージング
            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            float currentAngle = Mathf.Lerp(startAngle, endAngle, smoothT);

            // 座標と回転を計算して適用
            float rad = currentAngle * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f) * knifeRadius;

            knifeVisualObject.transform.position = transform.position + offset;
            knifeVisualObject.transform.rotation = Quaternion.Euler(0f, 0f, currentAngle - 45f);

            yield return null;
        }

        // 攻撃終了時に非表示＆判定もオフにする
        if (knifeSpriteRenderer != null) knifeSpriteRenderer.enabled = false;
        if (knifeCollider != null) knifeCollider.enabled = false;
        knifeVisualObject.SetActive(false);

        isAttacking = false;
    }
}