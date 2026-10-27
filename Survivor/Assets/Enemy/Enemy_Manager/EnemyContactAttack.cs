
using UnityEngine;

public class EnemyContactAttack : MonoBehaviour
{
    public float attackInterval = 1f;

    private EnemyStatus enemyStatus;
    private float nextAttackTime = 0f;

    void Awake()
    {
        enemyStatus = GetComponent<EnemyStatus>();
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

        if (enemyStatus == null)
        {
            Debug.LogError("EnemyStatus が見つかりません", gameObject);
            return;
        }

        PlayerStatus playerStatus =
            collision.gameObject.GetComponent<PlayerStatus>();

        if (playerStatus == null)
        {
            Debug.LogError("PlayerStatus が見つかりません");
            return;
        }

        playerStatus.TakeDamage(enemyStatus.Attack);

        nextAttackTime = Time.time + attackInterval;
    }
}
