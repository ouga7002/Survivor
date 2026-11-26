
using UnityEngine;
using System.Collections;

public class KnifeAttack : MonoBehaviour
{
    [Header("参照設定")]
    [SerializeField] private Transform aimCursor;
    [SerializeField] private GameObject knifeVisualObject;

    [Header("エフェクト設定")]
    [SerializeField] private GameObject knifeSlashEffectObject;

    [Header("攻撃パラメータ")]
    [Tooltip("ナイフ自体の基本攻撃力")]
    [SerializeField] private float baseWeaponAtk = 3f;

    [Tooltip("攻撃間隔（秒）")]
    [SerializeField] private float attackInterval = 2.0f;

    [Tooltip("斬撃にかかる時間")]
    [SerializeField] private float swingDuration = 0.25f;

    [Tooltip("カーソルよりどれくらい先で攻撃するか")]
    [SerializeField] private float distanceAheadOfCursor = 0.4f;

    [Tooltip("振る時のスイング角度幅")]
    [SerializeField] private float swingAngleRange = 90f;

    private float attackTimer = 0f;
    private bool isAttacking = false;

    private PlayerStatus playerStatus;
    private SpriteRenderer knifeSpriteRenderer;
    private Collider2D knifeCollider;

    private Animator knifeSlashAnimator;

    void Start()
    {
        playerStatus = GetComponent<PlayerStatus>();

        if (aimCursor == null)
        {
            GameObject cObj = GameObject.Find("AimCursor");
            if (cObj != null)
                aimCursor = cObj.transform;
        }

        // ナイフ本体の初期設定
        if (knifeVisualObject != null)
        {
            knifeSpriteRenderer = knifeVisualObject.GetComponent<SpriteRenderer>();
            knifeCollider = knifeVisualObject.GetComponent<Collider2D>();

            if (knifeSpriteRenderer != null)
                knifeSpriteRenderer.enabled = false;

            if (knifeCollider != null)
                knifeCollider.enabled = false;

            knifeVisualObject.SetActive(false);
        }

        // 斬撃エフェクトの初期設定
        if (knifeSlashEffectObject != null)
        {
            knifeSlashAnimator =
                knifeSlashEffectObject.GetComponent<Animator>();

            knifeSlashEffectObject.SetActive(false);
        }
    }

    void Update()
    {
        if (aimCursor == null) return;

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

    IEnumerator SwingAheadOfCursorRoutine()
    {
        if (knifeVisualObject == null || aimCursor == null)
            yield break;

        isAttacking = true;
        knifeVisualObject.SetActive(true);

        // ナイフ本体を表示
        if (knifeSpriteRenderer != null)
            knifeSpriteRenderer.enabled = true;

        if (knifeCollider != null)
            knifeCollider.enabled = true;

        // 斬撃エフェクトを再生
        if (knifeSlashEffectObject != null)
        {
            Vector3 direction =
                (aimCursor.position - transform.position).normalized;

            float effectAngle =
                Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // プレイヤーの位置に配置
            knifeSlashEffectObject.transform.position = transform.position;

            // カーソル方向に回転
            knifeSlashEffectObject.transform.rotation =
                Quaternion.Euler(0f, 0f, effectAngle);

            // エフェクトを表示
            knifeSlashEffectObject.SetActive(true);

            // アニメーションを最初から再生
            if (knifeSlashAnimator != null)
            {
                knifeSlashAnimator.Play("Knife_Slash", 0, 0f);
            }
        }

        float elapsed = 0f;

        while (elapsed < swingDuration)
        {
            if (aimCursor == null) break;

            elapsed += Time.deltaTime;
            float t = elapsed / swingDuration;

            Vector3 dirToCursor =
                (aimCursor.position - transform.position).normalized;

            float baseAngle =
                Mathf.Atan2(dirToCursor.y, dirToCursor.x) * Mathf.Rad2Deg;

            float cursorDist =
                Vector3.Distance(transform.position, aimCursor.position);

            float knifeRadius = cursorDist + distanceAheadOfCursor;

            float startAngle = baseAngle - (swingAngleRange / 2f);
            float endAngle = baseAngle + (swingAngleRange / 2f);

            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            float currentAngle =
                Mathf.Lerp(startAngle, endAngle, smoothT);

            float rad = currentAngle * Mathf.Deg2Rad;

            Vector3 offset = new Vector3(
                Mathf.Cos(rad),
                Mathf.Sin(rad),
                0f
            ) * knifeRadius;

            knifeVisualObject.transform.position =
                transform.position + offset;

            knifeVisualObject.transform.rotation =
                Quaternion.Euler(0f, 0f, currentAngle - 45f);

            yield return null;
        }

        // ナイフ本体を非表示にする
        if (knifeSpriteRenderer != null)
            knifeSpriteRenderer.enabled = false;

        if (knifeCollider != null)
            knifeCollider.enabled = false;

        knifeVisualObject.SetActive(false);

        // 斬撃エフェクトも非表示にする
        if (knifeSlashEffectObject != null)
            knifeSlashEffectObject.SetActive(false);

        isAttacking = false;
    }
}
