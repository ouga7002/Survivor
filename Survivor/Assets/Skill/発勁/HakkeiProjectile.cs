
using UnityEngine;
using System.Collections.Generic;

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

    // すでに攻撃した敵を記録
    private HashSet<KnockbackReceiver> hitEnemies =
        new HashSet<KnockbackReceiver>();

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        projectileCollider = GetComponent<Collider2D>();

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.collisionDetectionMode =
            CollisionDetectionMode2D.Continuous;

        projectileCollider.isTrigger = true;
    }

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

        rb.linearVelocity = direction * speed;

        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!initialized)
            return;

        // 敵のノックバックコンポーネントを取得
        KnockbackReceiver enemy =
            other.GetComponentInParent<KnockbackReceiver>();

        if (enemy == null)
            return;

        // 同じ敵に複数回当たらないようにする
        if (hitEnemies.Contains(enemy))
            return;

        // 石化中の敵は対象外
        Petrifiable petrifiable =
            enemy.GetComponent<Petrifiable>();

        if (petrifiable != null &&
            petrifiable.IsPetrified())
        {
            return;
        }

        // 攻撃済みとして記録
        hitEnemies.Add(enemy);

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

        // 衝撃波は消さずに飛び続ける
    }
}
