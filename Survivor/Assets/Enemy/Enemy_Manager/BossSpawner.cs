
using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    [System.Serializable]
    public class BossSpawnData
    {
        public GameObject bossPrefab;

        [Tooltip("ゲーム開始から何秒後に出現するか")]
        public float spawnTime = 120f;

        [HideInInspector]
        public bool hasSpawned = false;
    }

    [Header("ボスの出現設定")]
    public BossSpawnData[] bosses;

    [Header("プレイヤーからの出現距離")]
    public float minSpawnDistance = 7f;
    public float maxSpawnDistance = 10f;

    [Header("敵プール")]
    public EnemyPool enemyPool;

    private Transform player;
    private float elapsedTime;

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
    }

    void Update()
    {
        if (player == null || enemyPool == null) return;

        elapsedTime += Time.deltaTime;

        foreach (BossSpawnData data in bosses)
        {
            if (data.hasSpawned) continue;
            if (data.bossPrefab == null) continue;

            if (elapsedTime < data.spawnTime) continue;

            SpawnBoss(data);
            data.hasSpawned = true;
        }
    }

    void SpawnBoss(BossSpawnData data)
    {
        Vector2 direction =
            Random.insideUnitCircle.normalized;

        float distance =
            Random.Range(minSpawnDistance, maxSpawnDistance);

        Vector2 spawnPosition =
            (Vector2)player.position + direction * distance;

        enemyPool.Spawn(
            data.bossPrefab,
            spawnPosition
        );

        Debug.Log(data.bossPrefab.name + " が出現！");
    }
}

