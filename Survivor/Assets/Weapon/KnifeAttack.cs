using UnityEngine;
using System.Collections;

public class KnifeAttack : MonoBehaviour
{
    [Header("設定")]
    [Tooltip("プレイヤーのトランスフォーム")]
    [SerializeField] private Transform player;

    [Tooltip("エイムカーソルのトランスフォーム")]
    [SerializeField] private Transform aimCursor;

    [Tooltip("攻撃間隔（秒）")]
    [SerializeField] private float attackInterval = 2.0f;

    [Tooltip("攻撃（スイング）にかかる時間")]
    [SerializeField] private float swingDuration = 0.2f;

    [Tooltip("カーソル位置からの飛び出し・スイング幅")]
    [SerializeField] private float swingDistance = 0.4f;

    private float attackTimer = 0f;
    private SpriteRenderer spriteRenderer;
    private bool isAttacking = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // プレイヤーやカーソルの自動検索
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

        // 普段はナイフを非表示にしておく
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }
    }

    void Update()
    {
        if (player == null || aimCursor == null) return;

        // 攻撃中でないときは、タイマーを進める
        if (!isAttacking)
        {
            attackTimer += Time.deltaTime;

            if (attackTimer >= attackInterval)
            {
                attackTimer = 0f;
                StartCoroutine(SwingKnifeRoutine());
            }
        }
    }

    IEnumerator SwingKnifeRoutine()
    {
        isAttacking = true;

        // 攻撃開始時にナイフを表示
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }

        // カーソルの位置と、プレイヤーからカーソルへの方向を取得
        Vector3 cursorPosition = aimCursor.position;
        Vector3 directionToCursor = (cursorPosition - player.position).normalized;

        // 向き（角度）の計算
        float angle = Mathf.Atan2(directionToCursor.y, directionToCursor.x) * Mathf.Rad2Deg;

        float elapsed = 0f;

        // スイングモーション
        while (elapsed < swingDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / swingDuration;

            // カーソルの位置を基準にして、前後にスイングさせる
            float currentDist = Mathf.Sin(t * Mathf.PI) * swingDistance;
            transform.position = cursorPosition + directionToCursor * currentDist;

            // 刃が回転する「切ってる感じ」の演出
            float currentAngle = angle - 45f + (Mathf.Sin(t * Mathf.PI) * 60f);
            transform.rotation = Quaternion.Euler(0f, 0f, currentAngle);

            yield return null;
        }

        // 攻撃終了：ナイフを非表示に戻す
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        isAttacking = false;
    }
}