
using System.Collections;
using UnityEngine;

public class Boss1Controller : MonoBehaviour, IEnemyAttack
{
    public float moveSpeed = 2;

    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private EnemyAttackManager attackManager;

    [Header("弾のPrefab")]
    public GameObject bullet1;
    public GameObject bullet2;

    public Transform bulletSpawnPoint;
    public float spawnHeight = 1.0f;

    [Header("攻撃設定")]
    public float attackInterval = 4.5f;
    public float firstAttackDelay = 2f;

    [Header("直進弾の設定")]
    public int straightBulletCount = 16;
    public float straightBulletSpreadAngle = 150f;
    public float straightBulletSpawnDistance = 1.0f;

    [Header("攻撃予告設定")]
    public float warningDuration = 0.9f;
    public float flashInterval = 0.12f;

    [Header("ボス点滅用")]
    public SpriteRenderer warningSpriteRenderer;

    [Header("魔法陣")]
    public GameObject magicCircle;
    public SpriteRenderer magicCircleRenderer;
    public Color magicCircleColor = Color.yellow;
    public float magicCircleStartScale = 0.7f;
    public float magicCircleEndScale = 1.5f;

    private Coroutine attackCoroutine;
    private bool isAttacking = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        attackManager = GetComponent<EnemyAttackManager>();

        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
            player = playerObject.transform;

        if (warningSpriteRenderer != null)
        {
            warningSpriteRenderer.sprite = spriteRenderer.sprite;
            warningSpriteRenderer.enabled = false;
            warningSpriteRenderer.color =
                new Color(1f, 0.5f, 0f, 0.7f);
        }

        if (magicCircle != null)
            magicCircle.SetActive(false);

        attackCoroutine = StartCoroutine(AttackLoop());
    }

    IEnumerator AttackLoop()
    {
        yield return new WaitForSeconds(firstAttackDelay);

        while (isAttacking)
        {
            if (player != null)
            {
                // 予告する攻撃を先に決める
                bool useStraightAttack =
                    bullet1 != null &&
                    (bullet2 == null || Random.value < 0.5f);

                // 攻撃予告
                yield return StartCoroutine(ShowAttackWarning());

                if (!isAttacking)
                    yield break;

                // 予告終了後に攻撃
                if (player != null)
                {
                    if (useStraightAttack)
                        AttackStraight();
                    else
                        AttackHoming();
                }
            }

            // 次の予告開始までの時間を調整
            yield return new WaitForSeconds(
                Mathf.Max(0f, attackInterval - warningDuration));
        }
    }

    IEnumerator ShowAttackWarning()
    {
        float elapsed = 0f;
        bool flashVisible = false;

        if (magicCircle != null)
        {
            magicCircle.SetActive(true);
            magicCircle.transform.localScale =
                Vector3.one * magicCircleStartScale;
        }

        while (elapsed < warningDuration && isAttacking)
        {
            // ボス本体の点滅
            if (warningSpriteRenderer != null)
            {
                warningSpriteRenderer.sprite = spriteRenderer.sprite;
                warningSpriteRenderer.flipX = spriteRenderer.flipX;

                flashVisible = !flashVisible;
                warningSpriteRenderer.enabled = flashVisible;
            }

            // 魔法陣を徐々に大きくする
            float progress = elapsed / warningDuration;

            if (magicCircle != null)
            {
                float scale = Mathf.Lerp(
                    magicCircleStartScale,
                    magicCircleEndScale,
                    progress);

                magicCircle.transform.localScale =
                    Vector3.one * scale;
            }

            // 魔法陣を明滅させる
            if (magicCircleRenderer != null)
            {
                float alpha = Mathf.Lerp(
                    0.35f,
                    0.9f,
                    Mathf.PingPong(elapsed * 3f, 1f));

                magicCircleRenderer.color = new Color(
                    magicCircleColor.r,
                    magicCircleColor.g,
                    magicCircleColor.b,
                    alpha);
            }

            yield return new WaitForSeconds(flashInterval);
            elapsed += flashInterval;
        }

        HideAttackWarning();
    }

    void HideAttackWarning()
    {
        if (warningSpriteRenderer != null)
            warningSpriteRenderer.enabled = false;

        if (magicCircle != null)
            magicCircle.SetActive(false);
    }

    // 追尾弾攻撃
    void AttackHoming()
    {
        if (bullet2 == null) return;

        Vector3 createPosition;

        if (bulletSpawnPoint != null)
        {
            createPosition = bulletSpawnPoint.position;
        }
        else
        {
            createPosition = transform.position;
            createPosition.y += spawnHeight;
        }

        GameObject bullet = Instantiate(
            bullet2,
            createPosition,
            Quaternion.identity
        );

        if (attackManager != null)
            attackManager.RegisterBullet(bullet);
    }

    // 16方向の拡散弾攻撃
    void AttackStraight()
    {
        if (bullet1 == null || player == null) return;

        Vector2 direction =
            ((Vector2)player.position -
            (Vector2)transform.position).normalized;

        Vector2 spawnPosition =
            (Vector2)transform.position +
            direction * straightBulletSpawnDistance;

        for (int i = 0; i < straightBulletCount; i++)
        {
            float angle = 0f;

            if (straightBulletCount > 1)
            {
                float t =
                    (float)i / (straightBulletCount - 1);

                angle = Mathf.Lerp(
                    -straightBulletSpreadAngle,
                    straightBulletSpreadAngle,
                    t
                );
            }

            Vector2 shotDirection =
                Quaternion.Euler(0, 0, angle) * direction;

            GameObject bullet = Instantiate(
                bullet1,
                spawnPosition,
                Quaternion.identity
            );

            B1_Bullet1 straightBullet =
                bullet.GetComponent<B1_Bullet1>();

            if (straightBullet != null)
                straightBullet.Initialize(shotDirection);
            else
                Debug.LogError(
                    "Bullet1にB1_Bullet1が設定されていません！");

            if (attackManager != null)
                attackManager.RegisterBullet(bullet);
        }
    }

    void FixedUpdate()
    {
        if (player == null)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 direction =
            ((Vector2)player.position - rb.position).normalized;

        rb.linearVelocity = direction * moveSpeed;

        if (spriteRenderer != null)
        {
            if (direction.x > 0.01f)
                spriteRenderer.flipX = false;
            else if (direction.x < -0.01f)
                spriteRenderer.flipX = true;
        }
    }

    public void StopAttack()
    {
        isAttacking = false;

        if (attackCoroutine != null)
            StopCoroutine(attackCoroutine);

        HideAttackWarning();
    }
}
