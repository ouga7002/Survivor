using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("敵のステータス")]
    public float maxHP = 10f;

    [Header("ガチャポイントドロップ")]
    public int GPDropAmount = 1;

    [Header("撃破エフェクト")]
    public GameObject deathEffectPrefab;

    [Header("ドロップするオーブ")]
    public GameObject dropOrbPrefab;

    [Header("撃破スコア")]
    public int scoreReward = 10;

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

        // プレイヤーにスコアを加算
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            PlayerStatus playerStatus =
                playerObject.GetComponent<PlayerStatus>();

            if (playerStatus != null)
            {
                playerStatus.AddScore(scoreReward);
            }
        }

        // すべての敵の攻撃を停止
        EnemyAttackManager attackManager =
            GetComponent<EnemyAttackManager>();

        if (attackManager != null)
        {
            attackManager.StopAllAttacks();
        }

        // オーブを生成
        if (dropOrbPrefab != null && GPOrbPool.Instance != null)
        {
            GPOrbPool.Instance.Spawn(
                dropOrbPrefab,
                transform.position
            );
        }

        // 撃破エフェクトを生成
        if (EnemyDeathEffectPool.Instance != null)
        {
            EnemyDeathEffectPool.Instance.Spawn(transform.position);
        }

        // 敵をプールに戻す
        PooledEnemy pooledEnemy = GetComponent<PooledEnemy>();

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
