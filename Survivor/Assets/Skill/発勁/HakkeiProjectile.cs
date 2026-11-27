
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class HakkeiProjectile : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D projectileCollider;

    private Vector2 direction;
    private float speed;
    private float lifetime;

    private int damage;
    private float knockbackForce;
    private float knockbackDuration;

    private bool initialized = false;
    private bool hasHit = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        projectileCollider = GetComponent<Collider2D>();

        // 衝撃波の物理設定
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.collisionDetectionMode =
            CollisionDetectionMode2D.Continuous;

        // Trigger判定にする
        projectileCollider.isTrigger = true;
    }

    // Hakkei.csから設定を受け取る
    public void Initialize(
        Vector2 attackDirection,
        float projectileSpeed,
        float projectileLifetime,
        int attackDamage,
        float force,
        float duration)
    {
        direction = attackDirection.normalized;
        speed = projectileSpeed;
        lifetime = projectileLifetime;

        damage = attackDamage;
        knockbackForce = force;
        knockbackDuration = duration;

        initialized = true;

        // 衝撃波を飛ばす
        rb.linearVelocity = direction * speed;

        // 一定時間後に消滅
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!initialized || hasHit)
            return;

        // 敵のノックバックコンポーネントを取得
        KnockbackReceiver enemy =
            other.GetComponentInParent<KnockbackReceiver>();

        // 敵以外には反応しない
        if (enemy == null)
            return;

        // 石化中の敵には当たらない
        Petrifiable petrifiable =
            enemy.GetComponent<Petrifiable>();

        if (petrifiable != null &&
            petrifiable.IsPetrified())
        {
            return;
        }

        hasHit = true;

        // ダメージを与える
        enemy.gameObject.SendMessageUpwards(
            "TakeDamage",
            damage,
            SendMessageOptions.DontRequireReceiver
        );

        // 衝撃波の進行方向に吹き飛ばす
        enemy.ApplyKnockback(
            direction,
            knockbackForce,
            knockbackDuration
        );

        // 衝撃波を停止
        rb.linearVelocity = Vector2.zero;

        // 衝撃波を削除
        Destroy(gameObject);
    }
}
