using UnityEngine;

public class Boss1Controller : MonoBehaviour, IEnemyAttack
{
    public float moveSpeed = 2;

    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private EnemyAttackManager attackManager;

    public GameObject bullet1;
    public GameObject bullet2;

    // 弾の生成位置
    public Transform bulletSpawnPoint;

    // 生成位置が設定されていない場合の高さ
    public float spawnHeight = 1.0f;

    // 攻撃間隔
    public float attackInterval = 5f;

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
        if (bullet2 == null)
        {
            Debug.LogError("Bullet2が設定されていません！");
            return;
        }

        Vector3 createPosition;

        // 専用の生成位置が設定されている場合
        if (bulletSpawnPoint != null)
        {
            createPosition = bulletSpawnPoint.position;
        }
        else
        {
            // ボスの頭上に生成
            createPosition = transform.position;
            createPosition.y += spawnHeight;
        }

        GameObject bullet = Instantiate(
    bullet2,
    createPosition,
    Quaternion.identity
);

        // 生成した弾を共通管理に登録
        if (attackManager != null)
        {
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
