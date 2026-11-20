using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class GateSwordProjectile : MonoBehaviour
{
    [Header("剣の設定")]
    [SerializeField] private float swordSpeed = 12f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float lifeTime = 3f;

    private Vector2 moveDirection;

    private Rigidbody2D rb;
    private bool hasHit = false;

    // =========================================
    // 初期化
    // =========================================
    public void Initialize(
        Vector2 direction,
        float attackPower,
        Transform targetTransform = null)
    {
        // 射出時の方向を固定
        moveDirection = direction.normalized;

        // 攻撃力設定
        damage = attackPower;

        // targetTransformは使用しない
        // 射出後は敵を追尾しない

        // 自動消滅
        Destroy(gameObject, lifeTime);
    }

    // =========================================
    // 初期設定
    // =========================================
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    // =========================================
    // 移動
    // =========================================
    private void Update()
    {
        if (hasHit) return;

        // 射出時に決定した方向へ直進
        transform.position +=
            (Vector3)(moveDirection * swordSpeed * Time.deltaTime);

        // 剣の向きを移動方向に合わせる
        float angle = Mathf.Atan2(
            moveDirection.y,
            moveDirection.x
        ) * Mathf.Rad2Deg;

        transform.rotation =
            Quaternion.Euler(0f, 0f, angle);
    }

    // =========================================
    // 敵との衝突
    // =========================================
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit) return;

        // EnemyHealthを取得
        EnemyHealth enemyHealth =
            collision.GetComponent<EnemyHealth>();

        // Colliderが子オブジェクトの場合
        if (enemyHealth == null)
        {
            enemyHealth =
                collision.GetComponentInParent<EnemyHealth>();
        }

        // 敵以外は無視
        if (enemyHealth == null)
        {
            return;
        }

        // ダメージ処理
        enemyHealth.TakeDamage(damage);

        // 命中済みにする
        hasHit = true;

        // 剣を消滅
        Destroy(gameObject);
    }
}