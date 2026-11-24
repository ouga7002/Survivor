using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemySpawnData
    {
        public GameObject enemyPrefab;
        public float spawnInterval = 1f;

        [HideInInspector]
        public float nextSpawnTime;
    }

    [Header("敵ごとの出現設定")]
    public EnemySpawnData[] enemies;

    [Header("プレイヤーからの出現距離")]
    public float minSpawnDistance = 5f;
    public float maxSpawnDistance = 10f;

    [Header("敵の最大数")]
    public int maxEnemyCount = 100;

    [Header("敵プール")]
    public EnemyPool enemyPool;

    [Header("時間経過による出現頻度上昇")]
    public float timeStepSeconds = 60f;
    public float timeReduction = 0.1f;

    [Header("中ボス撃破による出現頻度上昇")]
    public float bossDefeatReduction = 0.2f;

    [Header("出現間隔の最低値")]
    public float minSpawnInterval = 0.3f;

    private Transform player;
    private float startTime;
    private int defeatedBossCount;

    void Start()
    {
        startTime = Time.time;

        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Playerタグのオブジェクトが見つかりません");
        }

        foreach (EnemySpawnData data in enemies)
        {
            data.nextSpawnTime =
                Time.time + GetCurrentSpawnInterval(data);
        }
    }

    void Update()
    {
        // ボス出現中は通常敵をスポーンしない
        if (BossSpawner.IsBossActive) return;

        if (player == null || enemyPool == null) return;

        foreach (EnemySpawnData data in enemies)
        {
            if (data.enemyPrefab == null) continue;

            if (Time.time < data.nextSpawnTime) continue;

            float currentInterval =
                GetCurrentSpawnInterval(data);

            // 最大数に達していたら出現を延期
            if (enemyPool.ActiveCount >= maxEnemyCount)
            {
                data.nextSpawnTime =
                    Time.time + currentInterval;

                continue;
            }

            SpawnEnemy(data);

            data.nextSpawnTime =
                Time.time + currentInterval;
        }
    }

    void SpawnEnemy(EnemySpawnData data)
    {
        Vector2 direction =
            Random.insideUnitCircle.normalized;

        float distance =
            Random.Range(minSpawnDistance, maxSpawnDistance);

        Vector2 spawnPosition =
            (Vector2)player.position + direction * distance;

        enemyPool.Spawn(
            data.enemyPrefab,
            spawnPosition
        );
    }

    // 現在の出現間隔を計算
    float GetCurrentSpawnInterval(EnemySpawnData data)
    {
        int elapsedSteps = 0;

        if (timeStepSeconds > 0f)
        {
            elapsedSteps =
                Mathf.FloorToInt(
                    (Time.time - startTime) / timeStepSeconds
                );
        }

        float reduction =
            elapsedSteps * timeReduction
            + defeatedBossCount * bossDefeatReduction;

        return Mathf.Max(
            minSpawnInterval,
            data.spawnInterval - reduction
        );
    }

    // 中ボス撃破時に呼び出す
    public void NotifyMidBossDefeated()
    {
        defeatedBossCount++;

        // 現在の出現予定にも新しい間隔を反映
        foreach (EnemySpawnData data in enemies)
        {
            if (data.enemyPrefab == null) continue;

            float currentInterval =
                GetCurrentSpawnInterval(data);

            data.nextSpawnTime = Mathf.Min(
                data.nextSpawnTime,
                Time.time + currentInterval
            );
        }

        Debug.Log(
            "中ボス撃破！敵の出現頻度が上昇しました。"
        );
    }
}
