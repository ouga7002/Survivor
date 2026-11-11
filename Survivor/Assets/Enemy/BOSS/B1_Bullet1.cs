using UnityEngine;

public class B1_Bullet1 : MonoBehaviour
{
    public float moveSpeed = 5f;

    [Header("弾の設定")]
    public float lifeTime = 10f;
    public float rotationOffset = 90f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private EnemyStatus enemyStatus;

    private Vector2 direction;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyStatus = GetComponent<EnemyStatus>();
    }

    void OnEnable()
    {
        // 生成されてから10秒後に消滅
        Destroy(gameObject, lifeTime);
    }

    // 発射方向を設定
    public void Initialize(Vector2 moveDirection)
    {
        direction = moveDirection.normalized;

        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        if (rb != null)
        {
            rb.linearVelocity = direction * moveSpeed;
        }

        // 画像を90度回転
        transform.rotation = Quaternion.Euler(
            0f,
            0f,
            rotationOffset
        );
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        // 設定された方向に直進
        rb.linearVelocity = direction * moveSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        PlayerStatus playerStatus =
            collision.GetComponent<PlayerStatus>();

        if (playerStatus != null && enemyStatus != null)
        {
            playerStatus.TakeDamage(enemyStatus.Attack);
        }

        Destroy(gameObject);
    }
}
