using UnityEngine;

public class Boss1Controller : MonoBehaviour, IEnemyAttack
{
    public float moveSpeed = 2;

    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private EnemyAttackManager attackManager;

    [Header("弾のPrefab")]
    public GameObject bullet1; // 3方向直進弾
    public GameObject bullet2; // 既存の追尾弾

    // 既存の追尾弾の生成位置
    public Transform bulletSpawnPoint;

    public float spawnHeight = 1.0f;

    [Header("攻撃設定")]
    public float attackInterval = 5f;

    [Header("3方向直進弾の設定")]
    public int straightBulletCount = 3;
    public float straightBulletSpreadAngle = 15f;
    public float straightBulletSpawnDistance = 1.0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        // 2秒後から5秒おきに攻撃
        InvokeRepeating(nameof(Attack), 2f, attackInterval);

        attackManager = GetComponent<EnemyAttackManager>();
    }

    void Attack()
    {
        if (player == null) return;

        // どちらの攻撃を使うかランダムに決定
        bool useStraightAttack =
            bullet1 != null &&
            (bullet2 == null || Random.value < 0.5f);

        if (useStraightAttack)
        {
            AttackStraight();
        }
        else
        {
            AttackHoming();
        }
    }

    // 既存の追尾弾攻撃
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
        {
            attackManager.RegisterBullet(bullet);
        }
    }

    // 3方向直進弾攻撃
    void AttackStraight()
    {
        if (bullet1 == null) return;

        // プレイヤーに向かう方向
        Vector2 direction =
            ((Vector2)player.position - (Vector2)transform.position).normalized;

        // ボスの目の前に生成
        Vector2 spawnPosition =
            (Vector2)transform.position +
            direction * straightBulletSpawnDistance;

        for (int i = 0; i < straightBulletCount; i++)
        {
            // 3発の角度を均等にずらす
            float angle = 0f;

            if (straightBulletCount > 1)
            {
                float t = (float)i / (straightBulletCount - 1);
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
            {
                straightBullet.Initialize(shotDirection);
            }
            else
            {
                Debug.LogError(
                    "Bullet1にB1_StraightBulletが設定されていません！");
            }

            if (attackManager != null)
            {
                attackManager.RegisterBullet(bullet);
            }
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

        // 左右の移動方向に合わせてスプライトを反転
        if (spriteRenderer != null)
        {
            if (direction.x > 0.01f)
            {
                spriteRenderer.flipX = false;
            }
            else if (direction.x < -0.01f)
            {
                spriteRenderer.flipX = true;
            }
        }
    }

    public void StopAttack()
    {
        CancelInvoke(nameof(Attack));
    }
}
