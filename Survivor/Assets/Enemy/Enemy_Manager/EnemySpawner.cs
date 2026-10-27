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

    private Transform player;

    void Start()
    {
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

        // 敵ごとに最初の出現時刻を設定
        foreach (EnemySpawnData data in enemies)
        {
            data.nextSpawnTime = Time.time + data.spawnInterval;
        }
    }

    void Update()
    {
        if (player == null) return;

        // 敵の最大数に達していたら出現させない
        if (GameObject.FindGameObjectsWithTag("Enemy").Length
            >= maxEnemyCount)
        {
            return;
        }

        // 敵ごとに独立してタイマーを確認
        foreach (EnemySpawnData data in enemies)
        {
            if (data.enemyPrefab == null) continue;

            if (Time.time >= data.nextSpawnTime)
            {
                SpawnEnemy(data);

                data.nextSpawnTime =
                    Time.time + data.spawnInterval;
            }
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

        Instantiate(
            data.enemyPrefab,
            spawnPosition,
            Quaternion.identity
        );
    }
}
