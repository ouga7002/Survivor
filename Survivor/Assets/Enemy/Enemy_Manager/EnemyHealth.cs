using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("敵のステータス")]
    public float maxHP = 10f;

    [Header("ガチャポイントドロップ")]
    public int GPDropAmount = 1;

    public float CurrentHP { get; private set; }

    private bool isDead = false;
    private PooledEnemy pooledEnemy;

    void Awake()
    {
        pooledEnemy = GetComponent<PooledEnemy>();
    }

    void OnEnable()
    {
        CurrentHP = maxHP;
        isDead = false;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        CurrentHP -= damage;

        if (CurrentHP < 0)
        {
            CurrentHP = 0;
        }

        if (CurrentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;

        // ガチャポイントを出現させる
        if (GPDropAmount > 0 &&
            GPOrbPool.Instance != null)
        {
            GPOrbPool.Instance.Spawn(
                transform.position,
                GPDropAmount
            );
        }

        // 敵をプールに返す
        if (pooledEnemy != null)
        {
            pooledEnemy.ReturnToPool();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
