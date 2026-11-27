using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("敵のステータス")]
    public float maxHP = 10f;

    [Header("ガチャポイントドロップ")]
    public int GPDropAmount = 1;

    [Header("オーブドロップ設定")]
    [SerializeField] private int dropOrbCountOnHit = 0;     // 被弾時に落とすオーブの数（普通の敵は0、レア敵は1など）
    [SerializeField] private int dropOrbCountOnDeath = 1;   // 撃破時に落とすオーブの数（レア敵は多めの5個など）
    [SerializeField] private float dropSpreadRadius = 1.0f; // オーブが散らばる範囲

    [Header("撃破エフェクト")]
    public GameObject deathEffectPrefab;

    [Header("ドロップするオーブ")]
    public GameObject dropOrbPrefab;

    [Header("撃破スコア")]
    public int scoreReward = 10;

    public float CurrentHP { get; private set; }

    private EnemyHitFlash hitFlash;

    private bool isDead = false;
    private PooledEnemy pooledEnemy;

    void Awake()
    {
        pooledEnemy = GetComponent<PooledEnemy>();
        hitFlash = GetComponent<EnemyHitFlash>();
    }

    void OnEnable()
    {
        CurrentHP = maxHP;
        isDead = false;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        // 被弾時に赤くする
        if (hitFlash != null)
        {
            hitFlash.Flash();
        }

        CurrentHP -= damage;

        // ★被弾時にオーブをドロップ（数が0より大きい場合のみ）
        if (dropOrbCountOnHit > 0)
        {
            DropOrbs(dropOrbCountOnHit);
        }

        if (CurrentHP < 0)
        {
            CurrentHP = 0;
        }

        if (CurrentHP <= 0)
        {
            Die();
        }
    }

    // 指定した数だけランダムな位置にオーブをスポーンさせる共通メソッド
    private void DropOrbs(int count)
    {
        if (dropOrbPrefab == null || GPOrbPool.Instance == null) return;

        for (int i = 0; i < count; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle * dropSpreadRadius;
            Vector3 spawnPos = transform.position + new Vector3(randomOffset.x, randomOffset.y, 0f);

            GPOrbPool.Instance.Spawn(dropOrbPrefab, spawnPos);
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

        // ★撃破時は設定された数のオーブを生成
        if (dropOrbCountOnDeath > 0)
        {
            DropOrbs(dropOrbCountOnDeath);
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