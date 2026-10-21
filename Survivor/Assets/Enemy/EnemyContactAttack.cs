using UnityEngine;

public class EnemyContactAttack : MonoBehaviour
{
    public float attackInterval = 1f;

    private Enemy1Controller enemy;
    private float nextAttackTime = 0f;

    void Start()
    {
        enemy = GetComponent<Enemy1Controller>();
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
        {
            return;
        }

        if (Time.time < nextAttackTime)
        {
            return;
        }

        if (enemy == null)
        {
            Debug.LogError("Enemy1Controller が見つかりません");
            return;
        }

        PlayerStatus playerStatus =
            collision.gameObject.GetComponent<PlayerStatus>();

        if (playerStatus == null)
        {
            Debug.LogError("PlayerStatus が見つかりません");
            return;
        }

        playerStatus.TakeDamage(enemy.Attack);

        nextAttackTime = Time.time + attackInterval;
    }
}