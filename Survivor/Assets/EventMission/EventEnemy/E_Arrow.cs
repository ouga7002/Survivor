
using UnityEngine;

public class E_Arrow : MonoBehaviour
{
    private Rigidbody2D rb;

    private Vector2 direction;
    private float damage;
    private bool hasHit;

    public void Initialize(
        Vector2 moveDirection,
        float speed,
        float arrowDamage,
        float lifeTime)
    {
        rb = GetComponent<Rigidbody2D>();

        direction = moveDirection.normalized;
        damage = arrowDamage;

        rb.linearVelocity = direction * speed;

        // 矢の向きを進行方向に合わせる
        float angle =
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation =
            Quaternion.Euler(0f, 0f, angle);

        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit)
            return;

        if (other.CompareTag("Player"))
        {
            PlayerStatus playerStatus =
                other.GetComponentInParent<PlayerStatus>();

            if (playerStatus != null)
                playerStatus.TakeDamage(damage);

            hasHit = true;
            Destroy(gameObject);
        }
    }
}

